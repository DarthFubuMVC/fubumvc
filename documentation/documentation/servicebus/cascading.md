<!--Title:Cascading Messages-->
<!--Url:cascading-->

Many times during the processing of a message you will need to create and send out other messages. Maybe you need to respond back to the original sender with a reply,
maybe you need to trigger a subsequent action, or send out additional messages to start some kind of background processing. You can do that by just having
your handler class use the `IServiceBus` interface as shown in this sample:

<[sample:NoCascadingHandler]>

The code above certainly works and this is consistent with most of the competing service bus tools. However, FubuMVC supports the concept of _cascading messages_
that allow you to automatically send out objects returned from your handler methods without having to use `IServiceBus` as shown below:

<[sample:CascadingHandler]>

When FubuMVC executes `CascadingHandler.Consume(MyMessage)`, it "knows" that the `MyResponse` return value should be sent through the 
service bus as part of the same transaction with whatever routing rules apply to `MyResponse`. A couple things to note here:

* Cascading messages returned from handler methods will not be sent out until after the original message succeeds and is part of the underlying
  transport transaction
* Null's returned by handler methods are simply ignored
* There is a significant performance advantage to using cascading messages instead of explicitly calling `IServiceBus.Send()` if you are using the
  LightningQueues transport
* The cascading message feature was explicitly designed to make unit testing handler actions easier by shifting the test strategy 
  to [state-based](http://blog.jayfields.com/2008/02/state-based-testing.html) where you mostly need to verify the state of the response
  objects instead of mock-heavy testing against calls to `IServiceBus`.


## Request/Reply Scenarios
 
Normally, cascading messages are just sent out according to the configured subscription rules for that message type, but there's
an exception case. If the original sender requested a response, FubuMVC will automatically send the cascading messages returned
from the action to the original sender if the cascading message type matches the reply that the sender had requested. 
If you're examining the `Envelope` objects for the message, you'll see that the "reply-requested" header
is "MyResponse."

Let's say that we have two running service bus nodes named "Sender" and "Receiver." If this code below
is called from the "Sender" node:

<[sample:Request/Replay-with-cascading]>

and inside Receiver we have this code:

<[sample:CascadingHandler]>

Assuming that `MyMessage` is configured to be sent to "Receiver," the following steps take place:

1. Sender sends a `MyMessage` message to the Receiver node with the "reply-requested" header value of "MyResponse"
1. Receiver handles the `MyMessage` message by calling the `CascadingHandler.Consume(MyMessage)` method
1. Receiver sees the value of the "reply-requested" header matches the response, so it sends the `MyResponse` object back to Sender
1. When Sender receives the matching `MyResponse` message that corresponds to the original `MyMessage`, it sets the completion back
   to the Task returned by the `IServiceBus.Request<TResponse>()` method


## Conditional Responses

You may need some conditional logic within your handler to know what the cascading message is going to be. If you need to return
different types of cascading messages based on some kind of logic, you can still do that by making your handler method return signature
be `object` like this sample shown below:

<[sample:ConditionalResponseHandler]>


## Delayed Messages

You may want to raise a delayed message by using the `DelayedResponse` class as shown below:

<[sample:DelayedResponseHandler]>

## Multiple Cascading Messages

You can also raise any number of cascading messages by returning either any type that can be
cast to `IEnumerable<object>`, and FubuMVC will treat each element as a separate cascading message.
An empty enumerable is just ignored.

<[sample:MultipleResponseHandler]>

## Send Message Back to the Sender

If you want to send a message right back to the original sender node, you can return your message
wrapped by the `RespondToSender` type:

<[sample:BackToSenderHandler]>


## Send Message to a Specific Channel

If you need to trigger a message that needs to be sent to a specific node and want to bypass the 
normal subscription routing, you can use the `SendDirectlyTo` wrapper like this:

<[sample:GoDirectlyHandler]>


## Fluent Interface for Controlling Responses

FubuMVC also supports a limited fluent interface to more exactly control how the cascading message
should be handled:

<[sample:RespondsHandler]>


## Custom Cascading Message Behavior

Finally, you can create your own custom response behavior by creating your own implementation of
`ISendMyself` and returning that wrapper object from your handler methods:

<[sample:ISendMyself-Specially]>

`DelayedResponse`, `RespondToSender`, and `SendDirectlyTo` are implementations of `ISendMyself`.

