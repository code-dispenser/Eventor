using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Scenarios;

public class LocalEventHandlers
{
    private readonly IEventAggregator _eventAggregator;

    public LocalEventHandlers(IEventAggregator eventAggregator)
    
        => _eventAggregator = eventAggregator;

    public async Task RunFireAndForget()
    {
        var basicEvent = new BasicEvent(nameof(RunFireAndForget));

        var subscription = _eventAggregator.Subscribe<BasicEvent>(FireAndForgetHandler);

        await Console.Out.WriteLineAsync($"Publishing the event {nameof(BasicEvent)}.");

        await _eventAggregator.Publish(basicEvent, PublishMethod.FireAndForget);

        await Console.Out.WriteLineAsync($"Pausing for 10ms as otherwise the method will have completed before the handler is finished as its fire and forget.");

        await Task.Delay(10);
        /*
             * If we dont dispose, as the methods run in quick sucession the GC will not have happened and the subscribed handler inside the singleton EventAggregator
             * will still be alive and be called again when the next BasicEvent is published in the next method run in the demo.
         */
        subscription.Dispose();
        /*
            * Only using an inline handler to keep all the code together.
        */ 
        async Task FireAndForgetHandler(BasicEvent basicEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Handling the event {nameof(BasicEvent)} in the local handler: {nameof(FireAndForgetHandler)}. \r\n");
        }
    }
  
    public async Task RunFireAndForgetWithUnhandledExceptions()
    {
        var basicEvent   = new BasicEvent(nameof(RunFireAndForgetWithUnhandledExceptions));
        var subscription = _eventAggregator.Subscribe<BasicEvent>(FireAndForgetErroneousHandler);

        Exception? caughtException = null;

        try
        {
            await Console.Out.WriteLineAsync($"Publishing the event {nameof(BasicEvent)} inside a try catch block, with an exception variable that is null.");

            await _eventAggregator.Publish(basicEvent, PublishMethod.FireAndForget);

            await Console.Out.WriteLineAsync($"Pausing for 10ms as otherwise the method will have completed before the handler is finished as its fire and forget.");

            await Task.Delay(10);
        }
        catch (Exception ex) { caughtException = ex; }

        await Console.Out.WriteLineAsync($"Finished processing, is the exception variable still null [{caughtException is null}] - captured type: [{caughtException?.GetType().FullName}]. \r\n");

        subscription.Dispose();

        async Task FireAndForgetErroneousHandler(BasicEvent basicEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Inside the the {nameof(FireAndForgetErroneousHandler)} and throwing an unhandled exception.");
            throw new NotImplementedException();
        }
    }


    public async Task RunWaitForAll()
    {
        var basicEvent = new BasicEvent(nameof(RunWaitForAll));

        var subscription = _eventAggregator.Subscribe<BasicEvent>(WaitForAllHandler);

        await Console.Out.WriteLineAsync($"Publishing the event {nameof(BasicEvent)}.");

        await _eventAggregator.Publish(basicEvent, PublishMethod.WaitForAll);

        /*
             * If we dont dispose as the methods run in quick sucession the GC will not have happened and the subscribed handler will still be alive
             * when the next method is run in the demo.
         */
        subscription.Dispose();

        async Task WaitForAllHandler(BasicEvent basicEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Handling the event {nameof(BasicEvent)} in the local handler: {nameof(WaitForAllHandler)}. \r\n");
        }
    }

    public async Task RunWaitForAllWithUnhandledExceptions()
    {

        var basicEvent = new BasicEvent(nameof(RunWaitForAllWithUnhandledExceptions));

        var subscription = _eventAggregator.Subscribe<BasicEvent>(WaitForAllWithUnhandledExceptions);

        Exception? caughtException = null;
        try
        {
            await Console.Out.WriteLineAsync($"Publishing the event {nameof(BasicEvent)} inside a try catch block, with an exception variable that is null.");

            await _eventAggregator.Publish(basicEvent,PublishMethod.WaitForAll);

            await Console.Out.WriteLineAsync($"Pausing for 10ms as otherwise the method will have completed before the handler is finished as its fire and forget.");
        }
        catch (Exception ex) { caughtException = ex; }

        await Console.Out.WriteLineAsync($"Finished processing, is the exception variable still null [{caughtException is null}] - captured type: [{caughtException?.GetType().FullName}]. \r\n");

        subscription.Dispose();

        async Task WaitForAllWithUnhandledExceptions(BasicEvent basicEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Inside the the {nameof(WaitForAllWithUnhandledExceptions)} and throwing an unhandled exception.");
            throw new NotImplementedException();
        }
    }

    

    



}
