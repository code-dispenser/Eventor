namespace Eventor.Core.Common.Seeds
{
    /// <summary>
    /// Interface that dynamic event handlers (classes that are registered with your chosen IOC) must implement
    /// in order to be invoked by the EventAggregator.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to handle. TEvent must be derived from EventBase</typeparam>
    public interface IEventHandler<TEvent> where TEvent : EventBase
    {
        /// <summary>
        /// Invoked by the EventAggregator to process the event.
        /// </summary>
        /// <param name="theEvent">The event to handle.</param>
        /// <param name="cancellationToken">Used to notify any handlers that a cancellation has been requested. The default is CancellationToken.None.</param>
        /// <returns>A task representing the completion of the event handling process.</returns>
        Task Handle(TEvent theEvent, CancellationToken cancellationToken);
    }
}
