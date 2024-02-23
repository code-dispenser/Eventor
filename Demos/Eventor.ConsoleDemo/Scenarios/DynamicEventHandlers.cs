using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Scenarios;

public class DynamicEventHandlers(IEventAggregator eventAggregator)
{
    private readonly IEventAggregator _eventAggregator = eventAggregator;

    public async Task RunFireAndForget()
    {
        var anotherBasicEvent = new AnotherBasicEvent(nameof(RunFireAndForget));

        await Console.Out.WriteLineAsync($"Publishing the event {nameof(AnotherBasicEvent)}.");

        await _eventAggregator.Publish(anotherBasicEvent, PublishMethod.FireAndForget);

        await Console.Out.WriteLineAsync($"Pausing for 10ms as otherwise the method will have completed before the dynamic handler is called as its fire and forget.");

        await Task.Delay(10);
    }

    public async Task RunWaitForAllWithUnhandledExceptions()
    {
        var yetAnotherBasicEvent = new YetAnotherBasicEvent(nameof(RunWaitForAllWithUnhandledExceptions));

        Exception? caughtException = null;

        try
        {
            await Console.Out.WriteLineAsync($"Publishing the event {nameof(AnotherBasicEvent)} inside a try catch block, with an exception variable that is null.");

            await _eventAggregator.Publish(yetAnotherBasicEvent, PublishMethod.WaitForAll);
        }
        catch (Exception ex) { caughtException = ex; }

        await Console.Out.WriteLineAsync($"Finished processing, is the exception variable still null [{caughtException is null}] - captured type: [{caughtException?.GetType().FullName}]. \r\n");

    }

}
