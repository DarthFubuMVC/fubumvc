<!--Title:Bootstrapping FubuMVC Applications-->
<!--Url:bootstrapping-->

<div class="alert alert-success">While the basic patterns shown here are likely to continue into Jasper, we will strive to improve the configuration syntax
inside of Jasper's equivalent to FubuRegistry.</div>

See also [Streamlining FubuMVC Bootstrapping & the Design Patterns Used](https://jeremydmiller.com/2015/08/19/streamlining-fubumvc-bootstrapping-the-design-patterns-used/)
from Jeremy's blog for more background. 

The 3.0 release made a lot of changes from older versions to attempt to streamline the complexity and performance of bootstrapping
applications. At this point, the entire configuration for your application is configured by a `FubuRegistry` -- either a custom class
of your own or by directly working with a `FubuRegistry` object. The runtime model and [facade](https://en.wikipedia.org/wiki/Facade_pattern) to a running FubuMVC application is 
the `FubuRuntime` class. 

## Boostrapping Simple Applications

If all you want to do is start up a new FubuMVC application with nothing but the default policies,
services, and endpoint discovery, use this simple call:

<[sample:simplest-possible-bootstrapping]>

The sample above enables the HTTP service support without any actual http server hosting, but doesn't
enable any of the service bus or job execution features. To spin up a simple application using the
service bus and job execution, use this syntax instead:

<[sample:simple-bus-bootstrapping]>

Finally, if you want to make a few alterations to the application setup but not enough to make you want to use the 
`FubuRegistry` mechanism explained in the next section, you can use a [nested closure](https://msdn.microsoft.com/en-us/magazine/Ee291514.aspx) in `FubuRuntime.Basic()` to
apply your customizations before the application is bootstrapped:

<[sample:configure-app-with-basic]>


## Using FubuRegistry for More Complicated Applications

<div class="alert alert-info">I strongly recommend using a custom FubuRegistry class in your application so that
you can easily reuse the same configuration in production hosting, development hosting, and in various 
automated testing harnesses.</div>

Any complex application is probably going to warrant a separate `FubuRegistry` class to describe your application
and the overrides, extensions, and settings for your application.

A custom `FubuRegistry` will look something like this one below:

<[sample:Bootstrapping-MyApplication]>

All the configuration is done inside of the constructor function of the class (or helper methods called from the constructor if you want).
`FubuRegistry` is an example of [object scoping](http://martinfowler.com/dslCatalog/objectScoping.html) where you attempt to simplify the syntax of an internal DSL by shortening
the calling chain. 

To bootstrap a `FubuRuntime` from your custom `FubuRegistry`, you have these options:

<[sample:using-fuburuntime]>



