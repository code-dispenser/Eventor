using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;
using Eventor.Core.Strategies;

namespace Eventor.ConsoleDemo.Scenarios
{
    public class CustomEventPublisher
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventPublisher  _eventPublisher;
        public CustomEventPublisher(IEventAggregator eventAggregator, IEventPublisher eventPublisher)
        
            => (_eventAggregator, _eventPublisher) = (eventAggregator,eventPublisher);
            
        public async Task RunCustomEventPublisher()
        {
            var customPublisherEvent = new CustomPublisherEvent(nameof(RunCustomEventPublisher));

            await _eventAggregator.Publish(customPublisherEvent,_eventPublisher);

        }



         
    }
}
