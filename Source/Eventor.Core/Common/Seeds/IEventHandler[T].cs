namespace Eventor.Core.Common.Seeds
{
    /// <summary>
    /// Inteface that dyanmic event handlers (classes that are registered with your chosen IOC) must implement
    /// in order to be invoked by the EventAggregator.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventHandler<TEvent>
    {
        /// <summary>
        /// Invoked by the EventAggrator to process the event.
        /// </summary>
        /// <param name="theEvent">The event to handle.</param>
        /// <param name="cancellationToken">Used to notify any handlers that a cancellation has been requested. The default is CancellationToken.None.</param>
        /// <returns>A task representing the completion of the event handling process.</returns>
        Task Handle(TEvent theEvent, CancellationToken cancellationToken);
    }
}
