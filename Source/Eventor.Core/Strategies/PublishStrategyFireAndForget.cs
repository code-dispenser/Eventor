using Eventor.Core.Common;
using Eventor.Core.Common.Seeds;

namespace Eventor.Core.Strategies;

/// <summary>
/// A strategy that just invokes the handler using Task.Run without waiting for the task to complete 
/// and/or to propagate an unhandled exceptions
/// </summary>
internal sealed class PublishStrategyFireAndForget : IEventPublisher
{
    ///<inheritdoc />
    public Task Publish<TEvent>(TEvent theEvent, List<TheEventHandler<TEvent>> handlers, CancellationToken cancellationToken) where TEvent : EventBase
    {
        List<Task> publishTasks = new();

        foreach (var handler in handlers)
        {
            _ = Task.Run(() => handler(theEvent, cancellationToken));
        }

        return Task.CompletedTask;
    }
}
