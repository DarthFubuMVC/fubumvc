<!--Title:Service Bus Applications-->
<!--Url:servicebus-->

<div class="alert alert-info">As of FubuMVC 3.0, what was FubuTransportation is now just a part of the main FubuMVC.Core library</div>

FubuMVC started as a framework for web applications with a strong focus on modularity, extensibility, and being unobstrusive
within your application code. Later on, we realized that FubuMVC's composable runtime pipeline would be a good fit for 
distributed messaging problems as well. From there, we added what was originally called "FubuTransportation" as an addon
to FubuMVC that added support for many distributed messaging patterns between .Net applications. Besides messaging, the service bus functionality
can be used locally just to provide asynchronous work processing with durable storage. 


## Terminology

* _Node_ - a running instance of FubuMVC with the service bus feature enabled. Do note that it's actually possible to run more than
  one FubuMVC application in the same process
* _Transport_ - a supported mechanism for sending messages between running FubuMVC nodes
* _Channel_ - a pathway to communicate with a running FubuMVC node that is backed by a transport strategy
* _Envelope_ - an object that wraps a message being sent or received by FubuMVC that adds header metadata and helps control
  how messages are sent and received. See the [Envelope Wrapper pattern](http://www.enterpriseintegrationpatterns.com/patterns/messaging/EnvelopeWrapper.html) for more information
* _Publish/Subscribe_ (pubsub) - messaging pattern that decouples the sending code from the routing to one or more recipients. One way communication from the sender
  to any receivers. See [Publish-Subscribe Channel](http://www.enterpriseintegrationpatterns.com/patterns/messaging/PublishSubscribeChannel.html) for more background.
* _Request/Reply_ - bi-directional messaging pattern where a request message to one node generates a response message back to the original sender
* _Behavior's_ - FubuMVC's middleware strategy
* _Handler's_ - a class that handles messages within a FubuMVC service bus application

## Service Bus Topics

<[TableOfContents]>