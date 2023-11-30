# Eventor

## Overview

Eventor serves as an event aggregator that blurs the lines between aggregator and mediator. It facilitates in-process event communication through the publisher-subscriber paradigm. Additionally, it allows the publication of events that can be processed by dynamically invoked handlers registered in an IOC container.

The library package can be seamlessly integrated into both web and desktop applications. Its inception was driven, in part, by the need for efficient in-process communication between components in Blazor Web Assembly, along with the capability to handle domain events on the backend server."

## Installation

Download and install the latest version of the [Eventor.Core](https://www.nuget.org/packages/Eventor.Core) package from [nuget.org](https://www.nuget.org/) using your preferred client tool.

## Example usage

The preferred approach is to register Eventor in your chosen IOC container as a singleton, thereby facilitating access to all of the functionality. However, if you do not need dynamically invoked event handlers, you can use Eventor without an IOC container.

Register the EventAggregator as preferred. Examples shown below use the Microsoft.Extensions.DependencyInjection package and Autofac IOC containers. These containers allow for both the injection of the event aggregator as a singleton service and the dynamic invocation of event handlers, which you can also register with the IOC container.
```
services.AddSingleton<IEventAggregator>(provider => new EventAggregator(type => provider.GetRequiredService(type)))

// Or

builder.Register(c =>
{
    var context = c.Resolve<IComponentContext>();
    return new EventAggregator(type => context.Resolve(type));
}).As<IEventAggregator>().SingleInstance();

```
**Note:** The **'EventAggregator'** has both a parameterless constructor and one that accepts a ```Func<Type,dynamic>```. This callback function allows Eventor, when publishing events, to request any IOC registered event handlers associated with the event being published. These handlers are then created and passed to an **'IEventPublisher'** implementation that will invoke the handler using its publish strategy.

A client and/or component can subscribe to receive events via local event handlers using the **'Subscribe'** method on the **'EventAggregator'**:

```
var eventSubscription = _eventAggregator.Subscribe<SomeEvent>(SomeEventHandler);

private async Task SomeEventHandler(SomeEvent theEvent, CancellationToken cancellationToken){\...\};
```
**Note:** Events are created by deriving from the **EventBase** abstract class. When you subscribe to receive events, you will be returned an **'EventSubscription'**. This subscription can be used to stop receiving events by calling its **'Dispose'** method. Eventor uses weak-referenced delegates.Once either the dispose method is called or the subscription is disposed or has gone out of scope, the underlying delegate handler will be removed from the managed invocation list.
A client and/or component can then raise an event using the **'Publish'** method, all subscribers and/or associated IOC registered event handlers will then be notified:

```
var someEvent = new SomeEvent(\..\); 
await _eventAggregator.Publish(someEvent, PublishMethod.FireAndForget, CancellationToken.None);
```
Events can be published using one of the two built-in publishing strategies, selected via the enum as shown above. Alternatively, you can provide your own publishing strategy by creating a concrete implementation of the **'IEventPublisher'** interface and passing it to the publish method.
For the built-in publishers, **'FireAndForget'** lives up to its name and will swallow any unhandled exceptions in any receiving event handlers. On the other hand, the **'WaitForAll'** option awaits a Task.WhenAll and, upon completion, will throw an **'AggregateException'** if one or more unhandled exceptions occurred in any of the receiving handlers.

In conjunction with the [documentation](https://github.com/code-dispenser/Eventor/wiki), it is recommended that you download the source code from the [Git repository](https://github.com/code-dispenser/Eventor) and explore the scenarios within the demo projects. These sample 
scenarios answer most of your questions.

Any feedback, positive or negative, is welcome, especially surrounding scenarios/usage.



