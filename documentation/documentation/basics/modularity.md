<!--Title:Modularity and Extensibility-->
<!--Url:modularity-->

<div class="alert alert-info">What's described here is what's remaining of the original 
<a href="https://github.com/FubuMvcArchive/bottles">Bottles</a> extensibility framework after
being merged into FubuMVC.Core. If you hear experienced FubuMVC users say the word "Bottle,"
that just refers to a FubuMVC extension.</div>

FubuMVC has strong mechanisms for extensibility and modularity that go well beyond most other web
or service bus frameworks. You can:
* Build extension assemblies that will be automatically pulled into the application at start up time
* Create reusable conventions or policies that can be imported into a FubuMVC application
* Author modular applications that implement features, "areas", or "slices" in separate projects

Extensions can include custom policies, new HTTP endpoints or message handlers, override or add application
service registrations to StructureMap, and every other possible configuration you can do with `FubuRegistry`.

See <[linkto:documentation/basics/policies]> for more information on custom policies.



## Extensions

To build a reusable extension for FubuMVC, utilize the `IFubuRegistryExtension` interface to express
the desired additions or changes to the application. Using our chain logging example from the previous section,
we could build this extension to both add the logging policy and override the built in logging with this:

<[sample:LoggingExtension]>

and add it to your application like this:

<[sample:ImportLoggingExtension]>


## Extension Assemblies

To mark an assembly as a valid FubuMVC extension, add an assembly level attribute called `FubuModule` to 
your assembly's `AssemblyInfo.cs` file (it doesn't have to be there, but that's idiomatic .Net).

<[sample:MarkingWithFubuModule]>

When a FubuMVC application is bootstrapped, it:
1. Searches for any assemblies from the application's bin directory
that are marked with the `[FubuModule]` assembly 
1. FubuMVC scans those assemblies for any public, concrete `IFubuRegistryExtension` class
1. Each `IFubuRegistryExtension` class is applied to the main `FubuRegistry` of the application
   being built up


## Slices/Areas with FubuPackageRegistry

<div class="alert alert-warning">Please don't think of this feature as an equivalent to ASP.Net MVC's
portable areas. There's <b>no coupling</b> from the main application to the extension and FubuMVC's extensibility
model shown here is far more powerful.</div>

FubuMVC has a built in mechanism to easily create vertical application slices using a built in form
of `IFubuRegistryExtension` called `FubuPackageRegistry`. To build your own, just write a public
subclass like this:

<[sample:FubuAppWithLoggingPolicy]>

<div class="alert alert-info">Make sure the constructor function of your package registry is public.</div>
