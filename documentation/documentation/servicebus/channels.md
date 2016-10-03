<!--Title:Channels and Transports-->
<!--Url:channels-->

The actual connectors in a FubuMVC service bus application are _channel's_ that are backed and created
by _transport's_. Today, FubuMVC only supports a transport based on the <[linkto:documentation/servicebus/lq;title=Lightning Queues]> project
and an in memory transport for testing.

## Installing Transport's

The only thing you need to do to add a transport type to FubuMVC is the presence of the assembly for the transport adapter.
In the case of the LightningQueues transport, you only need to install the FubuMVC.LightningQueues library via Nuget:

<pre>
Install-Package FubuMVC.LightningQueues
</pre>

## Configuring Channels

<div class="alert alert-info">
A key goal of the FubuMVC successor ("Jasper") is to simplify and streamline the service bus bootstrapping shown in this
section.
</div>

Channels require a little more work. First off, channels are identified and configured by a Uri matching the 
desired transport, port, and queue name. 

From the <[linkto:getting_started;title=Hello World]> example, we first need to build a <[linkto:documentation/settings;title="Settings" object]> for two channels
identified as `Pinger` and `Ponger`:

<[sample:HelloWorldSettings]>

All the `HelloWorldSettings` class is is a way to identify channels and act as a means to communicate the channel Uri's to
the service bus model. I'm hard coding the Uri's in the code above, but that information can be pulled from any kind of configuration using
the built in support for <[linkto:documentation/settings;title=strong typed configuration]> in FubuMVC.

To configure service bus channels, it's easiest to define your application with a `FubuTransportRegistry<T>` class where the "T"
is your settings class. Inside of your registry class, you need to use the fluent interface that hangs off of the `Channel()` method
to describe channels and their behavior. A pair of examples of this is shown below:

<[sample:PingApp]>

The `FubuTransportRegistry` is a subclass of the <[linkto:documentation/bootstrapping;title=FubuRegistry]> class that adds additional options 
germaine to the service bus feature.


## Listening to Messages from a Channel

While you can always send messages to any configured channel, you must explicitly mark which
channels should be monitored for incoming messages to the current node. That's done with the 
`ReadIncoming()` method shown below:

<[sample:ListeningApp]>


## Static Message Routing Rules

When you publish a message using `IServiceBus` without explicitly setting the Uri of the desired 
destination, FubuMVC has to invoke the known message routing rules and dynamic subscriptions to
figure out which locations should receive the message. Consider this code that publishes a
`PingMessage`:

<[sample:sending-messages-for-static-routing]>

To route `PingMessage` to a channel, we can apply static message routing rules directly on to a 
channel by using one of the _Accepts****_ methods as shown below:

<[sample:StaticRoutingApp]>

Do note that doing the message type filtering by namespace will also include child namespaces. In
our own usage we try to rely on either namespace rules or by using shared message assemblies. 

See also the <[linkto:documentation/servicebus/subscriptions]>

## Message Persistence

If the transport you're using supports this switch (the LightningQueues transport does), you can declare channels to publish messages
with either a delivery guaranteed, persistent strategy or by a non-persistent strategy. The non-guaranteed
delivery mode is significantly faster, but probably only suitable for message types where throughput
is more important than message reliability. 

Below is a sample of explicitly controlling the channel persistence:

<[sample:PersistentMessageChannels]>


## Control Queues

FubuMVC may need to send messages between running service bus nodes to coordinate activities, register for dynamic subscriptions,
or perform health checks. Some of these messages are time sensitive, so it will frequently be valuable to set up a separate
"control" channel for these messages so they aren't stuck in a backed up queue with your normal messages.

Also, the control messages fit the "fire and forget" messaging model, so we recommend using the non-persistent channel mode with these
channels.

Below is a sample of setting up a control channel:

<[sample:ControlChannelApp]>
