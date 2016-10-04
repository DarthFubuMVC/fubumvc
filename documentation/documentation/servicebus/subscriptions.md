<!--Title:Dynamic Subscriptions-->
<!--Url:subscriptions-->

<div class="alert alert-warning">If you're going to use dynamic subscriptions, we strongly recommend you use a separate "control" 
channel to separate the messages that FubuMVC sends between running nodes to coordinate subscriptions. We also strongly recommend using
some kind of durable subscription storage (explained below).</div>

In more advanced usages of the FubuMVC service bus, you may want your application to be able to
subscribe to messages produced by a separate application without having to first configure
the other application. The _subscriptions_ feature is the mechanism to make this registration. 

First off, remember that FubuMVC service bus applications can be run as a cluster of executing nodes. Knowing that,
subscriptions can be either:

* _Global_ - subscribe the entire cluster of nodes to specific messages published by another application
* _Local_ - subscribes **only** the executing node to specific messages published by another application

Use a global subscription to take advantage of load balancing between nodes. Use a local subscription if you want the subscribed
messages to be received by **every node** in the application cluster.


See also:
* <[linkto:documentation/servicebus/channels]> for information on static routing rules.
* <[linkto:documentation/servicebus/clustering]> for information on establishing clusters of cooperative nodes for a single application


## Configurating Subscriptions

Subscriptions are configured in the `FubuTransportRegistry<T>` class that establishes your application. 

First, assume you have a settings object for your service bus application that models both the incoming messages
channel for your application cluster and a channel to a completely separate application:

<[sample:NodeSettings]>

Now, to configure both a local subscription to each and every executing node and a global subscription for load balanced
messages to all the nodes:

<[sample:configuring-subscriptions]>

## How it works

FubuMVC exposes the idea of _subscription persistence_ to store information about the list of active applications, nodes, and subscriptions
in a durable storage of some sort. Between running nodes, FubuMVC has built in message types for `SubscriptionsChanged`, 
`SubscriptionRequested`, and `SubscriptionsRemoved` that it uses to coordinate subscriptions across running nodes and applications. 

When a FubuMVC service bus application is initialized, it:
* Persists information about the running node including where it's running and a Uri for a local reply channel to that specific node
* Loads the same information about any other nodes in the logical application cluster from the subscription storage
* Loads the list of previously persisted message subscriptions
* Sends out `SubscriptionRequested` messages to request dynamic subscriptions



## Customizing Subscription Storage

The default subscription storage is just an in memory model, so you'll almost certainly want to replace that with some sort of
durable storage mechanism if you're going to use subscriptions. To use a different subscription storage, you need an implementation
of the `ISubscriptionPersistence` interface that you can plug into the application services like so:

<[sample:SubscriptionStorageOverride]>

You can see a [sample implementation for RavenDb here](https://github.com/DarthFubuMVC/fubumvc/blob/2.2/src/FubuTransportation.RavenDb/RavenDbSubscriptionPersistence.cs).
There will be a version backed by [Marten](http://jasperfx.github.io/marten) soon as well.


The subscription storage was largely based on the assumption that you'd probably opt to use a document database for the backing store.
