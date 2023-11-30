using Eventor.Core.Common.Models;
using Eventor.Core.Strategies;

namespace Eventor.Core.Common.Seeds;
/*
    * Interface added primarily for xml documentation as not to add noise to the code where possible. Not used in tests or test IOC 
*/ 

/// <summary>
/// Event Aggregator is responsible for publishing events via notifiying all of the local subscribed event handlers and for communicating with the
/// chosen IOC container to obtain any registered event handlers that are dymaincall invoked.
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    /// Publishes the event via the chosen publishing stategy determined by the <typeparamref name="publishMethod"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that you wish to publish.</typeparam>
    /// <param name="theEvent">The event that you wish to publish.</param>
    /// <param name="publishMethod"></param>
    /// <param name="cancellationToken">Used to notify any handlers that a cancellation has been requested. The default is CancellationToken.None.</param>
    /// <returns>A task representing the completion of the event publishing process.</returns>
    Task Publish<TEvent>(TEvent theEvent, PublishMethod publishMethod = PublishMethod.FireAndForget, CancellationToken cancellationToken = default) where TEvent : EventBase;

    /// <summary>
    /// Publishes the event via the chosen publishing stategy, IEventPublisher.Publish.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that you wish to publish.</typeparam>
    /// <param name="theEvent">The event that you wish to publish.</param>
    /// <param name="eventPublisher">An object that implements IEventPublisher that will have its Publish method invoked <see cref="IEventPublisher"/>.</param>
    /// <param name="cancellationToken">Used to notify any handlers that a cancellation has been requested. The default is CancellationToken.None.</param>
    /// <returns>A task representing the completion of the event publishing process.</returns>
    Task Publish<TEvent>(TEvent theEvent, IEventPublisher eventPublisher, CancellationToken cancellationToken = default) where TEvent : EventBase;

    /// <summary>
    /// The Subscribe method allows you to subscribe to events that you can handle locally via handlers created in forms and view models, for example.
    /// Events can also be handled dynaically via registration of event handler classes in an IOC container.
    /// </summary>
    /// <typeparam name="TEvent">The type of event you wish to register the handler for.</typeparam>
    /// <param name="handler">The method that satifies the public delegate TheEventHandler&lt;Event&gt;
    /// The method must accept two arguments, the actual type of event and a CancellationToken.</param>
    /// <returns>An EventSubscription object <see cref="EventSubscription"/> to be disposed when you no longer wish to recieve events.</returns>
    EventSubscription Subscribe<TEvent>(TheEventHandler<TEvent> handler) where TEvent : EventBase;
}