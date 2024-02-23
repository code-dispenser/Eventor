using Eventor.Core.Common.Seeds;
using Eventor.Core.Strategies;

namespace Eventor.ConsoleDemo.Common.EventPublishers;
public class CustomPublishingStrategy : IEventPublisher
{
    public async Task Publish<TEvent>(TEvent theEvent, List<TheEventHandler<TEvent>> handlers, CancellationToken cancellationToken = default) where TEvent : EventBase
    {
        List<Exception> exceptions = [];

        foreach (var handler in handlers)
        {
            try
            {
                await Console.Out.WriteLineAsync($"In the custom event publisher that processes handlers one by one in sequence.");
                await Task.Run(() => handler(theEvent, cancellationToken), cancellationToken);
            }
            catch (Exception ex) { exceptions.Add(ex); }
        }

        if (exceptions.Count != 0) throw new AggregateException(exceptions);
    }
}
