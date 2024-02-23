using Eventor.Core.Common.Models;
using Eventor.Core.Common.Seeds;
using Eventor.Core.Tests.FixturesAndData.Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Eventor.Core.Tests.Unit
{
    public class EventAggregatorTests
    {
        private int _handledCount = 0;
        private int HandledCount => _handledCount;
        private void IncrementCount() => Interlocked.Increment(ref _handledCount);


        [Fact]
        public void Should_get_an_event_subscription_when_subscribing_to_receive_events()
        {
            var eventAggregator     = new EventAggregator();
            var theEvenSubscription = eventAggregator.Subscribe<BasicEvent>(BasicEventHandler);

            theEvenSubscription.Should().BeOfType<EventSubscription>();

            static async Task BasicEventHandler(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }

        }

        [Fact]
        public void Should_be_able_to_unsubscribe_from_an_event_by_calling_the_event_subscription_dispose_method()
        {
            var eventAggregator     = new EventAggregator();
            var theEvenSubscription = eventAggregator.Subscribe<BasicEvent>(BasicEventHandler);

            var theCountBeforeDispose = eventAggregator.HandlerCountFor<BasicEvent>();

            theEvenSubscription.Dispose();

            var theCountAfterDispose = eventAggregator.HandlerCountFor<BasicEvent>();

            using(new AssertionScope())
            {
                theCountBeforeDispose.Should().Be(1);
                theCountAfterDispose.Should().Be(0);
            }

            static async Task BasicEventHandler(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }

        }

        [Fact]
        public void The_same_event_handler_should_not_be_added_twice()
        {
            var eventAggregator       = new EventAggregator();
            var theEvenSubscription   = eventAggregator.Subscribe<BasicEvent>(BasicEventHandler);
            var duplicateSubscription = eventAggregator.Subscribe<BasicEvent>(BasicEventHandler);

            var theHandlerCount = eventAggregator.HandlerCountFor<BasicEvent>();

            theHandlerCount.Should().Be(1);

            static async Task BasicEventHandler(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }

        [Fact]
        public void Unsubscribing_from_a_duplicate_event_subscription_should_not_throw_an_exception()
        {
            var eventAggregator         = new EventAggregator();
            var theEvenSubscription     = eventAggregator.Subscribe<BasicEvent>(BasicEventHandler);
            var duplicateSubscription   = eventAggregator.Subscribe<BasicEvent>(BasicEventHandler);

            theEvenSubscription.Dispose();

            FluentActions.Invoking(() => duplicateSubscription.Dispose()).Should().NotThrow();

            static async Task BasicEventHandler(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }

        [Fact]
        public void The_weak_reference_comparer_should_return_true_for_identical_references()
        {
            var theComparer         = new EventAggregator.WeakReferenceDelegateComparer();
            var weakRefHandlerOne   = new WeakReference<Delegate>(BasicEventHandler);
            var weakRefHandlerTwo   = new WeakReference<Delegate>(BasicEventHandler);

            theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeTrue();

            static async Task BasicEventHandler(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }
        [Fact]
        public void The_weak_reference_comparer_should_return_false_for_different_references()
        {
            var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
            var weakRefHandlerOne = new WeakReference<Delegate>(BasicEventHandlerOne);
            var weakRefHandlerTwo = new WeakReference<Delegate>(BasicEventHandlerTwo);

            theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeFalse();

            async Task BasicEventHandlerOne(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
            async Task BasicEventHandlerTwo(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }

        [Fact]
        public void The_weak_reference_comparer_should_return_false_for_a_null_reference()
        {
            var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
            WeakReference<Delegate>? weakRefHandlerOne = null;
            var weakRefHandlerTwo = new WeakReference<Delegate>(BasicEventHandlerTwo);

            theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeFalse();

            static async Task BasicEventHandlerTwo(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }

        [Fact]
        public void The_weak_reference_comparer_should_return_false_for_a_null_reference_target()
        {
            var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
            WeakReference<Delegate>? weakRefHandlerOne = new WeakReference<Delegate>(null!);
            var weakRefHandlerTwo = new WeakReference<Delegate>(BasicEventHandlerTwo);

            theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeFalse();

            static async Task BasicEventHandlerTwo(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }

        [Fact]
        public void The_weak_reference_comparer_get_hash_code_should_return_an_integer_value()
        {
            var theComparer         = new EventAggregator.WeakReferenceDelegateComparer();
            var weakRefHandlerOne   = new WeakReference<Delegate>(BasicEventHandler);
            var weakRefHandlerTwo   = new WeakReference<Delegate>(BasicEventHandler);

            var theCodeValue = theComparer.GetHashCode(weakRefHandlerOne);

            using(new AssertionScope())
            {
                theCodeValue.Should().BeGreaterThan(0);
                theCodeValue.Should().Be(theComparer.GetHashCode(weakRefHandlerTwo));
            }

            static async Task BasicEventHandler(BasicEvent basicEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        }
        [Fact]
        public void The_weak_reference_comparer_get_hash_code_should_return_an_integer_value_when_target_is_null()
        {
            var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
            var weakRefHandlerOne = new WeakReference<Delegate>(null!);

            var theCodeValue = theComparer.GetHashCode(weakRefHandlerOne);
             theCodeValue.Should().BeGreaterThan(0);
        }
        [Fact]
        public void Trying_to_publish_a_null_event_should_throw_an_argument_null_exception()

            => FluentActions.Invoking(() => new EventAggregator().Publish<BasicEvent>(null!)).Should().ThrowExactlyAsync<ArgumentNullException>();
        
        [Fact]
        public void Trying_to_publish_an_event_with_a_null_publisher_should_throw_an_argument_null_exception()
           
            => FluentActions.Invoking(() => new EventAggregator().Publish<BasicEvent>(new BasicEvent("TheSender"),null!)).Should().ThrowExactlyAsync<ArgumentNullException>();

        [Fact]
        public async void Should_automatically_unsubscribe_if_subscription_is_null_or_out_of_scope()
        {
            var eventAggregator = new EventAggregator();
            var theEvent        = new BasicEvent("TheTester");
            var subscription    = eventAggregator.Subscribe<BasicEvent>(HandleBasicEvent);

            subscription = null;
            await Task.Delay(20);
            GC.Collect();

            await eventAggregator.Publish(theEvent, PublishMethod.WaitForAll);

            this.HandledCount.Should().Be(0);

            async Task HandleBasicEvent(BasicEvent theBasicEvent, CancellationToken cancellationToken)
            {
                this.IncrementCount();
                await Task.CompletedTask;
            }

        }

        [Fact]
        public async void Calling_dispose_on_the_returned_subscription_should_remove_the_handler()
        {
            var eventAggregator = new EventAggregator();
            var theEvent        = new BasicEvent("TheTester");
            var subscription    = eventAggregator.Subscribe<BasicEvent>(HandleBasicEvent);
            
            subscription.Dispose();

            await eventAggregator.Publish(theEvent, PublishMethod.WaitForAll);

            this.HandledCount.Should().Be(0);

            async Task HandleBasicEvent(BasicEvent theBasicEvent, CancellationToken cancellationToken)
            {
                this.IncrementCount();
                await Task.CompletedTask;
            }

        }
    }
}