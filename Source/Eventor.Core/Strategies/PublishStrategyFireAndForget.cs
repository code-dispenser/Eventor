﻿using Eventor.Core.Common;
using Eventor.Core.Common.Seeds;

namespace Eventor.Core.Strategies;

/// <summary>
/// A stategy that just invokes the handler using Task.Run without waiting for the task to complete 
/// and/or to propegate an unhandled exceptions
/// </summary>
internal class PublishStrategyFireAndForget : IEventPublisher
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