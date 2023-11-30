using Eventor.Core.Common.Seeds;
using Eventor.Core.Strategies;
using Eventor.Core.Tests.FixturesAndData.Events;
using FluentAssertions;
using Xunit;

namespace Eventor.Core.Tests.Unit.Stategies;

public class PublishStrategyWhenAllTest
{
    private int _handledCount     = 0;
    private int HandledCount      => _handledCount;
    private void IncrementCount() => Interlocked.Increment(ref _handledCount);

    [Fact]
    public async Task Should_throw_an_aggregate_exception_for_unhandled_exceptions_after_all_processing_is_completed()
    {
        var publishStrategy = new PublishStrategyWhenAll();
        var handlerList     = new List<TheEventHandler<BasicEvent>> { HandleBasicEvent, HandleBasicEvent2, HandleBasicEvent3, HandleBasicEvent4 };
        var theEvent        = new BasicEvent("TheTester");

         (await FluentActions.Invoking(() => publishStrategy.Publish(theEvent, handlerList, CancellationToken.None))
                             .Should().ThrowExactlyAsync<AggregateException>())
                             .And.InnerExceptions.Should().HaveCount(4);

        Task HandleBasicEvent(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            throw new Exception();
        }
        Task HandleBasicEvent2(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            throw new Exception();
        }
        async Task HandleBasicEvent3(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            throw new Exception();

        }
        async Task HandleBasicEvent4(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            throw new Exception();
        }
    }

    [Fact]
    public async Task Should_process_all_handlers_before_returning()
    {
        var publishStrategy = new PublishStrategyWhenAll();
        var handlerList     = new List<TheEventHandler<BasicEvent>> { HandleBasicEvent1, HandleBasicEvent2, HandleBasicEvent3, HandleBasicEvent4 };
        var theEvent        = new BasicEvent("TheTester");

        await publishStrategy.Publish(theEvent, handlerList, CancellationToken.None);

        HandledCount.Should().Be(4);

        Task HandleBasicEvent1(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
           this.IncrementCount();
            return Task.CompletedTask;
        }
        Task HandleBasicEvent2(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            return Task.CompletedTask;
        }
        async Task HandleBasicEvent3(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            await Task.CompletedTask;

        }
        async Task HandleBasicEvent4(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            await Task.CompletedTask;
        }
    }
}

