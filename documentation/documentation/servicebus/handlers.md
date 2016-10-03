<!--Title:Message Handlers-->
<!--Url:handlers-->

<div class="alert alert-info">It is perfectly valid usage to use more than one handler method for any message type. While the 
FubuMVC team doesn't necessarily recommend this in most cases, it may be useful for reusing some generic logic or creating
extensibility mechanisms in your application.</div>

Sending messages back and forth is all fine and good, but one way or another, _something_ has to actually handle and process
those messages. Inside of FubuMVC, message handling is done with _handler actions_. Handler actions are nothing more than
a public method on a public type that has a single input parameter for the message type that it processes:

<[sample:SimpleHandler]>

Note that there is absolutely no mandatory FubuMVC-specific interfaces or attributes or base types. One of FubuMVC's primary design
objectives from the beginning was to eliminate direct coupling from your application code to FubuMVC itself. FubuMVC has also
strived to reduce the amount of mandatory cruft in your application code like attributes, fluent interfaces, marker interfaces, and mandatory base classes that are 
so prevalent in many other frameworks in the .Net space.

<div class="alert alert-info">Jasper will support <i>method injection</i> to allow you to take in additional service dependencies
in addition to the message body itself in the handler methods to potentially simplify your application code.</div>

See also <[linkto:documentation/servicebus/cascading]> for other valid handler method signatures.


## Asynchronous Handlers

<div class="alert alert-warning">FubuMVC 3.0's async support is charitably described as "imperfect." FubuMVC 4.0 will be rolled out soon
with a truly "async all the way down" runtime model as a stopgap solution before "Jasper."</div>

If you're trying to be more efficient at runtime by taking advantage of asynchronous processing, you can make the signature
of your handler action methods return a `Task` or `Task<T>` if you're exposing a <[linkto:documentation/servicebus/cascading;title=cascading message]>.

A sample, async handler method is shown below:

<[sample:AsyncHandler]>


## Handler Dependencies

At runtime, the handler objects are created by the underlying [StructureMap](http://structuremap.github.io) container for your application, 
meaning that you can inject service dependencies into your handler objects:

<[sample:injecting-services-into-handlers]>

The handler objects are built by a [nested container](http://structuremap.github.io/the-container/nested-containers/) scoped to the current message.

See <[linkto:documentation/structuremap]> for more information.


## How FubuMVC Finds Handlers

FubuMVC uses [StructureMap 4.0's type scanning support](http://structuremap.github.io/registration/auto-registration-and-conventions/) to find 
handler classes and candidate methods from known assemblies based on naming conventions.


By default, FubuMVC is looking for public classes in the main application assembly with names matching these rules:

* Type name ends with "Handler"
* Type name ends with "Consumer"
* Type closes the open generic type `IStatefulSaga<T>` for classes implementing <[linkto:documentation/servicebus/sagas;title=sagas]>

From the types, FubuMVC looks for any public instance method that accepts a single parameter that is assumed to be the message type.

## Customize Handler Discovery

<div class="alert alert-warning">Do note that handler finding conventions are additive, meaning that adding additional handler sources doesn
not automatically disable the built in handler source. This is different than the action discovery in the HTTP side of FubuMVC.</div>

The easiest way to use the FubuMVC service bus functionality is to just code against the default conventions. However, if you wish to deviate
from those naming conventions you can either supplement the handler discovery or replace it completely with your own conventions.

At a minimum, you can disable the built in discovery, add a new handler source through the `FindBy()` methods, or register specific 
handler classes with the code below:

<[sample:CustomHandlerApp]>

If you want to build your own custom handler source, the easiest thing to do is to subclass the `HandlerSource` class from
FubuMVC and configure it in its constructor function like so:

<[sample:MyHandlerSource]>

If you want to go off the beaten path and do some kind of special handler discovery, you can directly implement
the `IHandlerSource` interface in your own code.

Let's say that you're converting an application to FubuMVC that previously used a service bus that exposed message handling through
an interface like `IHandler` shown below:

<[sample:IHandler_of_T]>

_An_ implementation of a custom action source that can discover `IHandler<T>` classes and methods may look like the following:

<[sample:MyCustomHandlerSource]>

Finally, you can direct FubuMVC to use your custom handler sources with code like this inside of your `FubuTransportRegistry`
class representing your application:

<[sample:AppWithCustomHandlerSources]>






## Subclass or Interface Handlers

FubuMVC will allow you to use handler methods that work against interfaces or abstract types to apply or reuse
generic functionality across messages. Let's say that some subset of your messages implement some kind of
`IMessage` interface like this one and an implentation of it below:

<[sample:Handlers-IMessage]>

You can handle the `MessageOne` specifically with a handler action like this:

<[sample:Handlers-SpecificMessageHandler]>

You can also create a handler for `IMessage` like this one:

<[sample:Handlers-GenericMessageHandler]>

When FubuMVC handles the `MessageOne` message, it first calls all the specific handlers for that message type,
then will call any handlers that handle a more generic message type (interface or abstract class most likely) where 
the specific type can be cast to the generic type. You can clearly see this behavior by examining the <[linkto:documentation/diagnostics;title=handler chain diagnostics]>.