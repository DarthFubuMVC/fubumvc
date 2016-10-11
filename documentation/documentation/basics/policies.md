<!--title: Conventions and Policies-->

TODO(More here!)

* Using BehaviorGraph
* Local v. Global
* Reordering policies
* Chain sources -- but not going to encourage folks to use this much anymore
* link to routing policies


## Custom policies

<div class="alert alert-info">Jasper will retain the "BehaviorGraph" configuration model, but will
eliminate the IActionBehavior interface objects in favor of inline, generated code to be much
more efficient than FubuMVC's current runtime pipeline.</div>

All the behavior chains (message handlers, HTTP endpoints, and job executions) are first modeled at 
application bootstrapping time through the <[linkto:documentation/basics/behaviors;title=behavioral model]> before
they are "baked" into the underlying StructureMap container. This design opens up a lot of opportunities to
extend FubuMVC applications by adding (or removing) extra behaviors to any chain.

Let's say that you want to log a simple message for each chain execution. You might first build a custom 
`IActionBehavior` like this:

<[sample:ActionLogger]>

To get that behavior applied before any HTTP endpoint action or message handler, we could write our own 
`IConfigurationAction` like this one:

<[sample:MyLoggingPolicy]>

To add the custom policy to your application, use this code:

<[sample:FubuAppWithLoggingPolicy]>