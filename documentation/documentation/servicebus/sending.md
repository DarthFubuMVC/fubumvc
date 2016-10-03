<!--title:Publishing Messages-->

FubuMVC's service bus is all about distributed messaging and it supports a couple different messaging patterns.

## Publish and Subscribe

FubuMVC supports the concept of "[publish/subscribe](https://en.wikipedia.org/wiki/Publish%E2%80%93subscribe_pattern)" publishing
that allows you to decouple the message sender from the downstream recipients. FubuMVC's service bus does this by finding
all the channels that are configured to receive the message type by a combination of the <[linkto:documentation/servicebus/channels;title=static routing rules]> 
and <[linkto:documentation/servicebus/subscriptions;title=dynamic subscriptions]>.

To send a message, simply call `IServiceBus.Send()` as shown below:

<[sample:Publishing-SendPing]>

## Publish and Wait for an Acknowledgement

To send a message and also know when the message has been successfully received and processed by the
downstream receiver, use the `IServiceBus.SendAndAwait()` method:

<[sample:Publishing-SendPingWithAck]>

## Sending to a Specific Destination

If you want to publish a message to a specific destination regardless of the routing rules,
you can specify the Uri in the overload of `IServiceBus.Send()` shown below:

<[sample:Publishing-SendDirectly]>


## Request and Reply

FubuMVC supports the [request/reply](https://en.wikipedia.org/wiki/Request%E2%80%93response) messaging pattern. In this usage,
you can send a message with the expectation of receiving a response from the downstream receiver.

First, the receiving handler will need to return a <[linkto:documentation/servicebus/cascading;title=cascaded message]> for the response time. Revisiting the ping pong
sample once again, a request/reply handler would look like this handler that accepts a `PingMessage` and sends back
a `PongMessage`:

<[sample:Publishing-PingHandler]>

On the sending side, you would use the `IServiceBus.Request<TResponse>(TRequest)` method:

<[sample:Publishing-RequestReply]>

Behind the scenes, FubuMVC uses envelope header values telling the receiver what response to send back and the reply
Uri of the original sender to send back the response. There's nothing else users need to do to opt into the request/reply
pattern.


## Using Envelope sender

Lastly, if you need to take complete control over how a message is sent and use options that aren't exposed in the
`IServiceBus` [facade](https://en.wikipedia.org/wiki/Facade_pattern), you can directly use the `IEnvelopeSender` service
directly and configure the <[linkto:documentation/servicebus/envelope;title=envelope wrapper]> yourself as shown in this example:

<[sample:Publishing-UsingEnvelopeSender]>


