namespace Eventor.Core.Common.Models;

/// <summary>
/// An event subscription that keeps the weak referenced delegate handler alive until disposed 
/// or the EventSubscription goes out of scope. 
/// Calling the Dispose method at any time unsubscribes the associated event handler.
/// </summary>
public class EventSubscription : IDisposable
{
    private bool _disposed = false;
    
    private readonly Action     _unsubscribeAction;
    private readonly Delegate   _eventHandler;

    public EventSubscription(Action unsubscribeAction, Delegate eventHandler)

        => (_unsubscribeAction, _eventHandler) = (unsubscribeAction, eventHandler);
    public void Dispose()
    {
        if (false == _disposed)
        {
            _disposed = true;
            _unsubscribeAction?.Invoke();
        }
    }
}
