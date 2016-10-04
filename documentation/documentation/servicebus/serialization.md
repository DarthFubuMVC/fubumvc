<!--Title:Message Serializers-->
<!--Url:serialization-->

<div class="alert alert-info">
The serialization mechanism and defaults are likely to change in Jasper. We will be deprecating the old Xml serialization and
adding some basic content negotiation to enable teams to start moving individual applications to more efficient serializers. Jil? Protobuffers?
To be determined.
<br /><br />
We also want to be able to configure serialization readers per incoming message type as a way of further decoupling applications without having
to share a common library of message DTO types.
</div>

To send a message, FubuMVC serializes the actual message body to a `byte[]` array before submitting
the message through a configured channel. Upon receipt of a message, FubuMVC will deserialize the
`byte[]` array back into the desired message body object. The deserialization is lazy and is evaluated
on the first access of `Envelope as shown below:

<[sample:lazy-envelope-serialization]>


## How a Serializer is Chosen

When a new message is about to be sent via a remote transport, FubuMVC selects a serializer based
on this progression:

1. If the `Envelope.ContentType` header is already set, choose a serializer matching that content type
1. If there is a default serializer for the channel a message is being sent to, use that serializer
1. Use the default serializer for the application

Do note that it's valid for FubuMVC to send the same message with a different serialization content type
to different channels in pub/sub scenarios.

## Explicitly Choose Serialization per Message

If you drop down to the lower level `IEnvelopeSender` service, you can explicitly choose the serializer
choice for a particular message. That usage is shown below:

<[sample:choose-a-content-type-when-sending]>


## Out of the Box Serializers

FubuMVC 3.0 has only a couple out of the box serializers:

* Xml serialization via a custom mechanism that was specifically used to maintain compatibility with the older
  Rhino Service Bus tooling
* Binary serialization using .Net's built in binary formatter
* JSON serialization via Newtonsoft.Json using the default JSON settings of the application


## Custom Serializers

You can plug a new serializer into FubuMVC by implementing the `IMessageSerializer` interface as shown below:

<[sample:MyCustomSerializer]>

See the next section for configuration options to use your custom serializer.

## Configuring Serialization Defaults

Below is some samples of adding custom serializers, configuring the default serialization for the application,
and establishing the default serialization for a specific channel.

<[sample:SerializedApp]>



