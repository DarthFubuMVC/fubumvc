using System;
using System.Threading.Tasks;
using Examples.HelloWorld.ServiceBus;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Runtime;

namespace Examples.ServiceBus
{
    public class Publishing
    {
        // SAMPLE: Publishing-SendPing
        public void SendPing(IServiceBus bus)
        {
            bus.Send(new PingMessage());
        }
        // ENDSAMPLE

        // SAMPLE: Publishing-SendPingWithAck
        public Task SendPingWithAck(IServiceBus bus)
        {
            return bus.SendAndWait(new PingMessage());
        }
        // ENDSAMPLE

        // SAMPLE: Publishing-SendDirectly
        public void SendDirectly(IServiceBus bus)
        {
            bus.Send(new Uri("lq.tcp://localhost:2245/pings"), new PingMessage());
        }
        // ENDSAMPLE

        // SAMPLE: Publishing-RequestReply
        public async Task RequestReply(IServiceBus bus)
        {
            var pong = await bus.Request<PongMessage>(new PingMessage());
        }
        // ENDSAMPLE

        // SAMPLE: Publishing-UsingEnvelopeSender
        public void UsingEnvelopeSender(IEnvelopeSender sender)
        {
            var envelope = new Envelope
            {
                Message = new PingMessage(),
                Destination = new Uri("lq.tcp://localhost:2300/pings"),
                ContentType = "text/xml"
            };

            sender.Send(envelope);
        }
        // ENDSAMPLE
    }

    // SAMPLE: Publishing-PingHandler
    public class PingHandler
    {
        public PongMessage Consume(PingMessage ping)
        {
            return new PongMessage();
        }
    }
    // ENDSAMPLE

}