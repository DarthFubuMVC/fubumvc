using Examples.HelloWorld.ServiceBus;
using FubuMVC.Core.ServiceBus;

namespace Examples.ServiceBus
{
    public class ConsumingMessages
    {
        // SAMPLE: consuming-messages
        public void consume(IServiceBus bus)
        {
            // Execute the entire chain for a message
            // synchronously and in the local application
            bus.Consume(new PingMessage());
        }
        // ENDSAMPLE
    }
}