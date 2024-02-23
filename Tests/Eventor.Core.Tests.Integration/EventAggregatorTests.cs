using Autofac;
using Eventor.Core.Common.Seeds;
using Eventor.Core.Tests.FixturesAndData.Events;
using Eventor.Core.Tests.FixturesAndData.Fixtures;
using FluentAssertions;
using Xunit;

namespace Eventor.Core.Tests.Integration;

public class EventAggregatorTests 
{
    public static int DynamicEventHandlerCounter { get; set; } = 0;

    private int _handledCount = 0;
    private int HandledCount => _handledCount;
    private void IncrementCount() => Interlocked.Increment(ref _handledCount);


    [Fact]
    public async Task Should_publish_by_fire_and_forget_strategy_when_chosen()
    { 
        EventAggregator eventAggregator = new();
        
        var basicEvent   = new BasicEvent("TheTester");
        var subscription = eventAggregator.Subscribe<BasicEvent>(HandleBasicEvent);

        await eventAggregator.Publish(basicEvent, PublishMethod.FireAndForget, CancellationToken.None);

        await Task.Delay(20);// wait for the handler its fire and forget otherwise the method would have exited.

        this.HandledCount.Should().Be(1);

        async Task HandleBasicEvent(BasicEvent basicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            await Task.CompletedTask;
        }

    }

    [Fact]
    public async Task Should_publish_by_wait_all_strategy_when_chosen()
    {
        EventAggregator eventAggregator = new();

        var basicEvent   = new BasicEvent("TheTester");
        var subscription = eventAggregator.Subscribe<BasicEvent>(HandleBasicEvent);

        await eventAggregator.Publish(basicEvent, PublishMethod.WaitForAll, CancellationToken.None);

        this.HandledCount.Should().Be(1);

        async Task HandleBasicEvent(BasicEvent basicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            await Task.CompletedTask;
        }
    }

    [Fact]
    public void Should_not_throw_when_publishing_an_event_without_any_handlers()
    {
        var eventAggregator = CreateDIEnabledEventAggregator();
        
        FluentActions.Invoking(() => eventAggregator.Publish(new BasicEvent("TheTester"), PublishMethod.WaitForAll, CancellationToken.None)).Should().NotThrowAsync();
    }

    [Fact]
    public void A_resolver_that_returns_null_for_a_type_should_produce_an_empty_list_of_handlers_with_throwing_an_exception()
    {
        var eventAggregator = new EventAggregator(t => null!);

        FluentActions.Invoking(() => eventAggregator.Publish(new BasicEvent("TheTester"), PublishMethod.WaitForAll, CancellationToken.None)).Should().NotThrowAsync();
    }

    [Fact]
    public async Task Registered_dynamic_event_handlers_should_be_called_for_the_associated_published_event()
    {
        var eventAggregator = CreateDIEnabledEventAggregator();
        var basicEvent      = new BasicEvent("TheTester");

        DynamicEventHandlerCounter = 0;

        await eventAggregator.Publish(basicEvent, PublishMethod.WaitForAll, CancellationToken.None);

        DynamicEventHandlerCounter.Should().Be(1);
    }

    [Fact]
    public async Task The_publish_method_should_set_the_events_publish_time_in_ticks()
    {
        var     theBasicEvent   = new BasicEvent("TheTester");
        var     eventAggregator = new EventAggregator();
        Int64   theTickCount    = 0;

        var eventSubscription = eventAggregator.Subscribe<BasicEvent>(SomeEventHandler);

        await eventAggregator.Publish(theBasicEvent, PublishMethod.WaitForAll, CancellationToken.None);

        theTickCount.Should().BeGreaterThan(0);

        async Task SomeEventHandler(BasicEvent theEvent, CancellationToken cancellationToken)
        {
            theTickCount = theEvent.PublishTimeTicks;
            await Task.CompletedTask;
        }


    }

    public class MyInternalIntegrationTestHandler : IEventHandler<BasicEvent>
    {
        public async Task Handle(BasicEvent theEvent, CancellationToken cancellationToken)
        {
            DynamicEventHandlerCounter++;
            await Task.CompletedTask;
        }
    }

    private EventAggregator CreateDIEnabledEventAggregator()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<MyInternalIntegrationTestHandler>().As<IEventHandler<BasicEvent>>().InstancePerDependency();
        builder.Register<EventAggregator>(c =>
        {
            var context = c.Resolve<IComponentContext>();
            return new EventAggregator(type => context.Resolve(type));
        }).SingleInstance();

        IContainer container = builder.Build();

        return container.Resolve<EventAggregator>();
    }
}
