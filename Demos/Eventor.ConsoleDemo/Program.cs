using Autofac;
using Eventor.ConsoleDemo.Common.DynamicEventHandlers;
using Eventor.ConsoleDemo.Common.EventPublishers;
using Eventor.ConsoleDemo.Common.Events;
using Eventor.ConsoleDemo.Scenarios;
using Eventor.Core;
using Eventor.Core.Common.Seeds;
using Eventor.Core.Common.Validation;
using Eventor.Core.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Eventor.ConsoleDemo;

internal class Program
{
    static async Task Main()
    {
        
        try
        {
            var serviceProvider              = FromConfiguredMicrosoftContainer();
            var eventAggregator              = serviceProvider.GetRequiredService<IEventAggregator>();
            var customEventPublisherScenario = serviceProvider.GetRequiredService<CustomEventPublisher>();
            /*
                * Or for Autofac, comment out above and uncomment below. 
             */ 
            //var serviceProvider = FromConfiguredAutofacContainer();
            //var eventAggregator = serviceProvider.Resolve<IEventAggregator>();
            //var customEventPublisherScenario = serviceProvider.Resolve<CustomEventPublisher>();

            var lineSeparator = new String('-', 100) + "\r\n";

            await Console.Out.WriteLineAsync("Running local event handlers using fire and forget and wait for all publishing methods");
            await Console.Out.WriteLineAsync(lineSeparator);

            var localEventHandlers = new LocalEventHandlers(eventAggregator);

            await localEventHandlers.RunFireAndForget();
            await localEventHandlers.RunFireAndForgetWithUnhandledExceptions();
            await localEventHandlers.RunWaitForAll();
            await localEventHandlers.RunWaitForAllWithUnhandledExceptions();

            await Console.Out.WriteLineAsync("Running dynamic event handlers using fire and forget and wait for all publishing methods");
            await Console.Out.WriteLineAsync("They work exactly the same as the local handlers except that you cannot unsubscribe as they are dynamic/reactive");
            await Console.Out.WriteLineAsync(lineSeparator);

            var dynamicEventHandlers = new DynamicEventHandlers(eventAggregator);

            await dynamicEventHandlers.RunFireAndForget();
            await dynamicEventHandlers.RunWaitForAllWithUnhandledExceptions();

            await Console.Out.WriteLineAsync("Running both handler types using fire and forget");
            await Console.Out.WriteLineAsync(lineSeparator);

            await new BothHandlerTypes(eventAggregator).RunHandlersUsingFireAndForget();

            //Give the fire and forget time to finish otherwise the console messages will get muddled.
            await Task.Delay(10);

            await Console.Out.WriteLineAsync("Using a custom publisher injected in to the scenario");
            await Console.Out.WriteLineAsync(lineSeparator);

            var subscription = eventAggregator.Subscribe<CustomPublisherEvent>(LocalCustomPublisherEvenHandler);
            await customEventPublisherScenario.RunCustomEventPublisher();

            static async Task LocalCustomPublisherEvenHandler(CustomPublisherEvent theEvent, CancellationToken cancellationToken)
            {
                await Console.Out.WriteLineAsync($"Handled the event {nameof(CustomPublisherEvent)} in the local handler {nameof(LocalCustomPublisherEvenHandler)}.");
            }
        }

        catch (Exception ex)
        {
           await Console.Out.WriteLineAsync(ex.Message);
        }

        Console.ReadLine();
    }

    private static ServiceProvider FromConfiguredMicrosoftContainer()
    {
        var serviceProvider = Host.CreateApplicationBuilder()
                                    .Services
                                        .AddTransient<CustomEventPublisher>()
                                        .AddSingleton<IEventPublisher,CustomPublishingStrategy>()
                                        .AddTransient<IEventHandler<AnotherBasicEvent>, AnotherBasicEventHandler>()
                                        .AddTransient<IEventHandler<YetAnotherBasicEvent>, YetAnotherBasicEventHandlerThatThrowsExceptions>()
                                        .AddTransient<IEventHandler<OrderProcessedEvent>, OrderProcessedEventHandler>()
                                        .AddSingleton<IEventAggregator>(provider => new EventAggregator(type => provider.GetRequiredService(type)))
                                    .BuildServiceProvider();

        return serviceProvider;
    }

    private static IContainer FromConfiguredAutofacContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<CustomEventPublisher>().AsSelf().InstancePerDependency();
        builder.RegisterType<CustomPublishingStrategy>().As<IEventPublisher>().SingleInstance();
        builder.RegisterType<AnotherBasicEventHandler>().As<IEventHandler<AnotherBasicEvent>>().InstancePerDependency();
        builder.RegisterType<YetAnotherBasicEventHandlerThatThrowsExceptions>().As<IEventHandler<YetAnotherBasicEvent>>().InstancePerDependency();
        builder.RegisterType<OrderProcessedEventHandler>().As<IEventHandler<OrderProcessedEvent>>().InstancePerDependency();
        builder.Register(c =>
        {
            var context = c.Resolve<IComponentContext>();
            return new EventAggregator(type => context.Resolve(type));
        }).As<IEventAggregator>().SingleInstance();

        return builder.Build();

    }
}
