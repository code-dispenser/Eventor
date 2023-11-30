using Eventor.Core.Common.Models;
using Eventor.Core.Common.Seeds;
using Eventor.Core.Common.Validation;
using Eventor.Core.Strategies;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Eventor.Core
{
    ///<inheritdoc />
    public class EventAggregator : IEventAggregator
    {
        private readonly ConcurrentDictionary<Type, List<WeakReference<Delegate>>>  _eventSubscriptions = new();
        private readonly ConcurrentDictionary<PublishMethod, IEventPublisher>       _publishMethods = new();

        private readonly Func<Type, dynamic>? _resolver;

        public EventAggregator() { InitPublishers(); }

        /// <summary>
        /// Overload version of the constructor that accepts a func as a call back mechanism to facilitate
        /// communications with the IOC container for dynamic event handlers.
        /// </summary>
        /// <param name="resolver">A callback used to communicate with the chosen IOC container.</param>
        public EventAggregator(Func<Type, dynamic> resolver)
        {
            _resolver = resolver;
            InitPublishers();
        }

        private void InitPublishers()
        {
            _publishMethods[PublishMethod.FireAndForget] = new PublishStrategyFireAndForget();
            _publishMethods[PublishMethod.WaitForAll]    = new PublishStrategyWhenAll();
        }

        ///<inheritdoc />
        public EventSubscription Subscribe<TEvent>(TheEventHandler<TEvent> handler) where TEvent : EventBase
        {
            var eventType       = typeof(TEvent);
            var handlerList     = _eventSubscriptions.GetOrAdd(eventType, _ => new());
            var weakRefHandler  = new WeakReference<Delegate>(handler);
            var comparer        = new WeakReferenceDelegateComparer();

            if (false == handlerList.Any(existing => comparer.Equals(existing, weakRefHandler))) handlerList.Add(weakRefHandler);

            return new EventSubscription(() => Unsubscribe(eventType, weakRefHandler), handler);
        }
        private void Unsubscribe(Type eventType, WeakReference<Delegate> handler)
        {
            if (true == _eventSubscriptions.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);

                if (handlers.Count == 0) _eventSubscriptions.TryRemove(eventType, out _);
            }
        }

        ///<inheritdoc />
        public async Task Publish<TEvent>(TEvent theEvent, PublishMethod publishMethod = PublishMethod.FireAndForget, CancellationToken cancellationToken = default) where TEvent : EventBase
        
            => await Publish(theEvent, _publishMethods[publishMethod], cancellationToken);

        ///<inheritdoc />
        public async Task Publish<TEvent>(TEvent theEvent, IEventPublisher eventPublisher, CancellationToken cancellationToken = default) where TEvent : EventBase
        {
            _ = Check.ThrowIfNull(theEvent);
            _ = Check.ThrowIfNull(eventPublisher);

            theEvent.PublishTimeTicks = Stopwatch.GetTimestamp();

            var handlers = new List<TheEventHandler<TEvent>>();

            handlers.AddRange(GetSubscribedHandlers<TEvent>());

            if (_resolver != null) handlers.AddRange(GetRegisteredHandlers<TEvent>(cancellationToken));

            await eventPublisher.Publish(theEvent, handlers, cancellationToken).ConfigureAwait(false);
        }
        private List<TheEventHandler<TEvent>> GetRegisteredHandlers<TEvent>(CancellationToken cancellationToken) where TEvent : EventBase
        {
            List<TheEventHandler<TEvent>> registeredHandlers = new();

            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));
            var enumerableHandlersType = typeof(IEnumerable<>).MakeGenericType(eventHandlerType);
            var registeredEventHandlers = _resolver!(enumerableHandlersType) as Array;

            if (registeredEventHandlers == null) return registeredHandlers;

            foreach (var eventHandler in registeredEventHandlers!)
            {
                TheEventHandler<TEvent> handler = (TEvent, _) => (eventHandler as IEventHandler<TEvent>)!.Handle(TEvent, cancellationToken);

                registeredHandlers.Add(handler);

            }

            return registeredHandlers;
        }

        private List<TheEventHandler<TEvent>> GetSubscribedHandlers<TEvent>() where TEvent : EventBase
        {
            List<TheEventHandler<TEvent>> eventHandlers = new();

            List<WeakReference<Delegate>> subscribedHandlers;

            subscribedHandlers = _eventSubscriptions.TryGetValue(typeof(TEvent), out subscribedHandlers!) == true ? subscribedHandlers.ToList() : new List<WeakReference<Delegate>>();

            foreach (var subscribedHandler in subscribedHandlers)
            {
                if ((subscribedHandler.TryGetTarget(out Delegate? target)) && (target is TheEventHandler<TEvent> handler))
                {
                    eventHandlers.Add(handler);
                }
                else { Task.Run(() => Unsubscribe(typeof(TEvent), subscribedHandler)); }
            }

            return eventHandlers;
        }

        //Helper for testing
        internal int HandlerCountFor<TEvent>()
        {
            var eventType = typeof(TEvent);

            return _eventSubscriptions.ContainsKey(eventType) ? _eventSubscriptions[eventType].Count : 0;
        }
        internal class WeakReferenceDelegateComparer : IEqualityComparer<WeakReference<Delegate>>
        {
            public bool Equals(WeakReference<Delegate>? x, WeakReference<Delegate>? y)
            {
                if (x == null || y == null) return false;

                Delegate? xTarget, yTarget;
                if (false == x.TryGetTarget(out xTarget) || false == y.TryGetTarget(out yTarget)) return false;

                return xTarget == yTarget || xTarget.Equals(yTarget);
            }

            public int GetHashCode(WeakReference<Delegate> obj)
            {
                Delegate? target;
                return obj.TryGetTarget(out target) && target != null ? target.GetHashCode() : obj.GetHashCode();
            }
        }
    }
}

