using Eventor.Core.Common.Seeds;

namespace Eventor.Core.Strategies;
/// <summary>
/// An interface to allow custom event publishers to be used for the publising of events.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes the event by invoking all handlers associated with the event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="theEvent">The actual event to publish</param>
    /// <param name="handlers">The list of handlers for the type of event. Contains both subscription based handlers and IOC registered handlers.</param>
    /// <param name="cancellationToken">Used to notify any handlers that a cancellation has been requested. The default is CancellationToken.None.</param>
    /// <returns>A task representing the completion of the publishing process.</returns>
    Task Publish<TEvent>(TEvent theEvent, List<TheEventHandler<TEvent>> handlers, CancellationToken cancellationToken = default) where TEvent : EventBase;
}
