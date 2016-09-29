<!--Title:Getting Started-->
<!--Url:getting_started-->

The first step is to install the FubuMVC.Core nuget. That should bring down dependencies on:

* FubuMVC.Core itself
* StructureMap 4.* (as of 3.0, FubuMVC eliminated its IoC abstractions and only runs on StructureMap)
* Newtonsoft.Json, so watch for binding conflict issues. Sigh.
* HtmlTags 2.1 -- FubuMVC is incompatible with newer versions of HtmlTags
* FubuCore

## Web Applications

FubuMVC was originally a framework for building web applications, and that's still the easiest, default way to
use FubuMVC. To get started with the classic hello world exercise, start a new class library and add this class
below:

<[sample:HelloWorld-HomeEndpoint]>

Based on the default conventions within FubuMVC, the `HomeEndpoint.Index()` method will be used to handle
the root "/" url of your web application. The next step would be to bootstrap your new application using the
`FubuRuntime` class:

<[sample:HelloWorld-Bootstrapping]>

In the code above, we're starting up a basic application with all the defaults for the current Assembly. 
The `FubuRuntime` is the key class that represents a running FubuMVC application.

Now, to actually exercise this endpoint, we can use FubuMVC's "Scenario" support to write a unit test below:

<[sample:HelloWorld-Running]>

Finally, we can start up our hello world application with the [NOWIN web server](https://github.com/Bobris/Nowin) in a self hosted
console application like this:

<[sample:HelloWorld-self-host]>

Do note that you would need to install the NOWIN nuget separately.


## ServiceBus Applications

FubuMVC was originally a framework for web applications and the service bus functionality was added originally in
an add on called [FubuTransportation](https://github.com/FubuMvcArchive/FubuTransportation) and later merged into
FubuMVC 3.0. As such, the service bus takes a little more work to bootstrap.

For "hello, world" with the ServiceBus, let's just do a "ping pong" exercise where one running node sends a "Ping" messages
to another running node, which sends back a "Pong" message reply to the original node.

First, let's create our two message types and the actual message handler classes that will process the message
types:

<[sample:HelloWorld-handlers-and-messages]>

So a couple things to note about the code above:

* The message types need to be serializable by whatever serializer that you're using within the service bus
* `PingHandler` and `PongHandler` don't need to implement any kind of FubuMVC interface or base class
* FubuMVC will find the handler classes by its type scanning conventions 

Now that we've got messages and related handlers, we need to figure out how messages are going to get back an forth via "channels" backed
by a "transport" mechanism. FubuMVC's main transport is based on [LightningQueues]()https://github.com/LightningQueues/LightningQueues (LQ)
and requires the extra FubuMVC.LightningQueues library from Nuget.

The nice thing about the LightningQueues transport is that it doesn't require any installation. All we need to do
to enable the LQ backed channels is to configure the queue name and port we want our application to use. Assuming that
we'll have two channels we're going to name "Pinger" and "Ponger," we'll build out a Settings class that will hold the addresses
of our two channels:

<[sample:HelloWorldSettings]>

After all that, we're finally ready to build out our two applications. The applications are expressed by
a subclass of the `FubuTransportRegistry<T>` class, where "T" is the Settings type from above.

<[sample:PingApp]>

Finally, we can spin up both the "Ping" and "Pong" applications and send messages from one to the other:

<[sample:send_and_receive]>
