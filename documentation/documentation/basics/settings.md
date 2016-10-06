<!--Title: "Settings" Classes for Strong-Typed Configuration-->

See [How We Do Strong Typed Configuration](https://jeremydmiller.com/2014/11/07/strong_typed_configuration/) from Jeremy's blog
for more context on FubuMVC's configuration strategy.

FubuMVC is built with the idea of strong-typed configuration objects for runtime information like file paths, connection strings,
and any kind of information you'd generally opt to put in external configuration sources like an app.config/web.config file in
.Net.

In effect, that means building your own application configuration in a _Settings_ class like so:

<[sample:MyAppSettings]>

These _Settings_ classes are injected into the underlying StructureMap container as part of application bootstrapping. 
To consume the Settings objects directly into your code, you can either access them directly by using service location
from the application container, or better yet, just use constructor injection in your application code (assuming that the
object is built with StructureMap in the first place):

<[sample:injecting-settings]>

If you absolutely have to do it this way, you can also pull settings objects out of the underlying container:

<[sample:retrieve-settings]>

Be aware that the Settings objects are scoped as singletons within the StructureMap container.

Why did we do it this way and what are the advantages?

* Effectively decouples FubuMVC itself and your application from the mandatory presence of any kind of configuration file
* Improves traceability between configuration items and what code depends on that configuration
* Makes your application's configuration dependencies more apparent by just scanning the Settings classes
* Helps in automated testing or even just alternative deployment scenarios by being able to inject in different configuration at runtime or
  source the configuration from something other than the built in `System.Configuration` techniques
* Enables FubuMVC to apply configuration overrides from the application or extensions


## External Configuration with System.Configuration

Behind the scenes, you can have the data resolution of each property in your Settings class built
by using data from the old [appSettings](https://msdn.microsoft.com/en-us/library/610xe886.aspx) section in an app.config file like this:


```
<?xml version="1.0" encoding="" ?>
<configuration>
    <appSettings>
        <add key="MyAppSettings.FileShare" value="~/fileshare" />
    </appSettings>
</configuration>

```

The naming convention is to make the key be "[class name].[property name]." There are, of course, a few caveats:

* The settings convention binding from appSettings can only work with properties, but not public fields. That will be changed in Jasper.
* The underlying data binding can handle any kind of primitive type (strings, enums, numbers, dates, booleans) and any other type that 
  can be converted by the same [FubuCore](https://github.com/DarthFubuMVC/fubucore) model binding used for form posts inside of FubuMVC.
* It's not shown here - and very rarely used now - but it is possible to make the settings provider work with different data sources than
  the `appSettings` in .Net configuration.
* Your Settings class needs to have a no argument, public constructor function to be resolved from the model binding

<div class="alert alert-info">
Do note that the appSettings information, if any exists, will be applied to the Settings object when it is first created. That holds true for both
Settings objects configured inside of a FubuRegistry, used by FubuMVC itself, or if they are lazily built by StructureMap at runtime.
</div>

<div class="alert alert-warning">
But what if I want to have two different Settings classes with the same name but in different namespaces? Yeah, just don't do that.
</div>


## Settings Precedence

In order of decreasing precedence, properties in a Settings object are determined by:

1. `AlterSettings()` calls in your `FubuRegistry` class for the application
1. `AlterSettings()` calls in `IFubuRegistryExtension` classes for loaded extension assemblies
1. Information read from `appSettings` in your system's config file
1. Default values from the Settings classes themselves -- and this gets a lot cleaner with C# 6


See also <[linkto:documentation/basics/modularity]> for information.



## Altering Settings in FubuRegistry

You can programmatically alter Settings objects in code as part of bootstrapping with code like this:

<[sample:using-alter-settings]>

In some cases, you will need to use this mechanism for altering built in Settings classes. 





## Plans for Jasper

This general strategy has been very successful (in my humble opinion) and will most certainly continue
on to Jasper, but will be built upon the [newer configuration model in ASP.Net Core](https://docs.asp.net/en/latest/fundamentals/configuration.html) 
instead of the creaky old `System.Configuration` namespace. The replacement will possibly be an extension
library for [StructureMap](http://structuremap.github.io) that _should_ be perfectly usable outside of Jasper too.

We will probably support the `IOptions<T>` model from ASP.Net Core for compatibility, but Jeremy thinks that that approach is "meh."