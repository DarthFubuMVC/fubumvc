<!--Title:Integration with StructureMap for IoC-->

<div class="alert alert-info">If you're wondering why FubuMVC uses StructureMap as its sole
IoC tool, it's because <a href="http://jeremydmiller.com">Jeremy Miller</a> is the primary author of both tools.
Owning the entire stack has been convenient on a couple different occasions for performance optimizations.</div>

FubuMVC no longer supports any IoC tool other than [StructureMap](http://structuremap.github.io). We may change that in Jasper,
but for now, being StructureMap-only allowed us to greatly simplify and shrink down FubuMVC's internals for the 3.0 release.
Moreover, FubuMVC uses [StructureMap's optimized type scanning](http://structuremap.github.io/registration/auto-registration-and-conventions/) for its own internal bootstrapping.

## Configuring Services

First off, let's say that you have your own service with an interface plus an implementation that you want
to explicitly register in the underlying StructureMap container:

<[sample:SM-your-own-services]>

To add this service to the underlying application container, use the `FubuRegistry.Services` property as shown below:

<[sample:SM-inline-configuration]>

The `FubuRegistry.Services` property is just a StructureMap `Registry` with some additional convenience methods
for very common usages in FubuMVC applications.

If your application is even remotely complicated though, we recommend using a separate `Registry` class
for your custom services like this:

<[sample:SM-using-a-registry]>

All of the service registration is done through [StructureMap's Registry DSL mechanism](http://structuremap.github.io/registration/registry-dsl/).

## At Runtime

When a chain is executed at runtime, StructureMap builds or resolves the entire object graph of the various 
<[linkto:documentation/basics/behaviors;title=behaviors]>. That includes:

* <[linkto:documentation/servicebus/handlers;title=Message handlers]>
* <[linkto:documentation/http/endpoints;title=Endpoints]>
* `IActionBehavior`'s
* Any dependencies of the classes listed above

To put that into some perspective, you can utilize StructureMap to inject your own service dependencies,
FubuMVC services, and even request or message specific services too:

<[sample:SM-can-inject-dependencies-into-endpoint]>

<div class="alert alert-info">The proposed Jasper architecture will reduce and simplify
the usage of StructureMap at runtime by trying to drastically reduce the number of objects built by
StructureMap and attempting to make much more of the service resolution lazy.</div>


## Per Request or Message Nested container

When FubuMVC executes any behavior chain by handling an HTTP request, processing a message in the service bus, or executing a job, it utilizes
StructureMap's [nested container](http://structuremap.github.io/the-container/nested-containers/) feature to
both manage the scoping of services and automatically handle the disposal of any service created during a 
chain execution. Unless a service is registered with a non-default lifecycle, you can safely assume that it is
scoped to the request/message/job execution.

## Service disposal

As mentioned in the previous section, StructureMap automatically tracks and disposes any objects created directly
by the nested container as part of chain execution. Otherwise, services registered as singleton scoped are disposed
when the StructureMap `IContainer` is disposed, which in turn is disposed as part of disposing a `FubuRuntime`.



## Overriding System Dependencies

It is perfectly valid and possible to override FubuMVC's own services
in your application. While this has been frequently useful, do this with some caution.

Maybe the most common usage of this is to replace FubuMVC's built in JSON serialization mechanism.
The out of the box serialization uses [Newtonsoft.Json](http://www.newtonsoft.com/json), but what
if you want to use an alternative like [Jil](https://github.com/kevin-montrose/Jil) instead? 

Assuming that you have a class named `MyJilJsonSerializer` that implements FubuMVC's built in `IJsonSerializer`
interface, you can use your own service like so:

<[sample:SM-override-system-service]>
