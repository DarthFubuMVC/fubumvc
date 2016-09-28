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