<!--Title:Executing Commands-->

If all you need to do is to execute the full behavior chain for a message, in process, and **right now**,
you can use the `IServiceBus.Consume()` method as shown below:

<[sample:consuming-messages]>

This mechanism is frequently useful for automated integration testing. Do note that FubuMVC will throw an exception
if the running application does not have a handler chain for that message type.