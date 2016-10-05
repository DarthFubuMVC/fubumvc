<!--Title:The LightningQueues Transport-->
<!--Url:lq-->

<div class="alert alert-info">The LightningQueues transport will be part of the default service bus Nuget in the "Jasper" timeframe, and
Jasper may directly utilize LightningDB for default subscription persistence and delayed message processing.</div>

<div class="alert alert-warning">
Do note that only one process on a machine can listen to messages to a single port. If you try to run multiple LQ-connected applications
on your local development system, just ensure that each application is listening to a different port.
</div>

The primary transport for FubuMVC is based off of [LightningQueues](https://github.com/LightningQueues/LightningQueues) (LQ). LightningQueues is
a persistent, store and forward queuing library for .Net applications. The latest version of LQ uses [LightningDB](https://github.com/CoreyKaylor/Lightning.NET) 
for persistence instead of the slower, more problematic Esent storage in previous versions. The huge advantage of LightningQueues is that it's 
completely _xcopy_. No installation or configuration is necessary other than having the right binaries and a Uri designating queue names and IP
ports.

The only thing you need to do to enable the LQ transport in FubuMVC is to install the FubuMVC.LightningQueues in your application, typically via Nuget.

To opt into LightningQueues in a service bus application, use a Uri that follows this pattern:

```
lq.tcp://[machine name]:[port]/[queue name]
```

_lq.tcp_ is just the Uri scheme that designates an LQ endpoint. The machine name can be either a remote server, an IP address, or "localhost" for
local only development. 

Here's an example of configuring a service bus application with LightningQueues backed channels:

<[sample:LqApp]>

A couple things to note about the sample above:

* When FubuMVC constructs channels at application bootstrapping time, it matches the Uri scheme name ("lq.tcp" in this case) to the transport
  and asks that transport to build a channel for that Uri. 
* The `DeliveryFastWithoutGuarantee()` mode directs LQ to **not** try to persist messages before attempting to 
  send them to a remote location. This is an appropriate mode when performance is paramount or messages are quickly
  obsolete. This is somewhat comparable to [ZeroMQ](http://zeromq.org/).
* Behind the scenes, LQ makes all queues at a single port be either all persistent or not persistent. FubuMVC
  will throw exceptions at runtime if you try to run two queues to the same port with different persistence profiles.



