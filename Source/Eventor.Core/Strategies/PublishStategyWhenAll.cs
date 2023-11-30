using Eventor.Core.Common.Seeds;

namespace Eventor.Core.Strategies;

/// <summary>
/// A stategy that awaits Task.WhenAll to complete and returns an AggregateException containing an unhandled exceptions.
/// and/or to propegate an unhandled exceptions
/// </summary>
public class PublishStrategyWhenAll : IEventPublisher
{
    ///<inheritdoc />
    public async Task Publish<TEvent>(TEvent theEvent, List<TheEventHandler<TEvent>> handlers, CancellationToken cancellationToken) where TEvent : EventBase
    {
        List<Task> eventTasks = new();

        foreach (var handler in handlers)
        {
            eventTasks.Add(Task.Run(() => handler(theEvent, cancellationToken))); //Exception handling works for both async Task and non async Task handlers i.e inside the returned task}
        }

        try
        {
            await Task.WhenAll(eventTasks);
        }
        catch
        {
            throw new AggregateException(eventTasks.Where(t => t.Exception != null).Select(e => e.Exception!));
        }
    }
}
