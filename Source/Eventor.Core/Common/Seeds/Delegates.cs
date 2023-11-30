namespace Eventor.Core.Common.Seeds;

/// <summary>
/// Delegate used for both locally subscribed handlers and IOC registered classes (dynamic event handlers).
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
/// <param name="theEvent">The event to handle.</param>
/// <param name="cancellationToken">Used to notify any handlers that a cancellation has been requested. The default is CancellationToken.None.</param>
/// <returns></returns>
public delegate Task TheEventHandler<TEvent>(TEvent theEvent, CancellationToken cancellationToken) where TEvent : EventBase;
