using System;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using Shouldly;
using Xunit;

namespace Examples.HelloWorld.ServiceBus
{
    // SAMPLE: HelloWorldSettings
    public class HelloWorldSettings
    {
        public Uri Pinger { get; set; } = 
            "lq.tcp://localhost:2352/pinger".ToUri();

        public Uri Ponger { get; set; } =
            "lq.tcp://localhost:2353/ponger".ToUri();
    }
    // ENDSAMPLE

    // SAMPLE: PingApp
    public class PingApp : FubuTransportRegistry<HelloWorldSettings>
    {
        public PingApp()
        {
            // Configuring PingApp to send PingMessage's
            // to the PongApp
            Channel(x => x.Ponger)
                .AcceptsMessage<PingMessage>();

            // Listen for incoming messages from "Pinger"
            Channel(x => x.Pinger)
                .ReadIncoming();
        }
    }

    public class PongApp : FubuTransportRegistry<HelloWorldSettings>
    {
        // Listen for incoming messages from "Ponger"
        public PongApp()
        {
            Channel(x => x.Ponger)
                .ReadIncoming();
        }
    }
    // ENDSAMPLE

    // SAMPLE: send_and_receive
    public class HelloWorld
    {
        [Fact]
        public async Task send_and_receive()
        {
            // Spin up the two applications
            var pinger = FubuRuntime.For<PingApp>();
            var ponger = FubuRuntime.For<PongApp>();

            var bus = pinger.Get<IServiceBus>();

            // This sends a PingMessage and waits for a corresponding
            // PongMessage reply
            var pong = await bus.Request<PongMessage>(new PingMessage());

            pong.ShouldNotBe(null);

            pinger.Dispose();
            ponger.Dispose();
        }
    }
    // ENDSAMPLE

    // SAMPLE: HelloWorld-handlers-and-messages
    public class PingMessage
    {
    }

    public class PongMessage
    {
    }

    public class PongHandler
    {
        public void Handle(PongMessage pong)
        {
            Console.WriteLine("Received pong message");
        }
    }

    public class PingHandler
    {
        // This is an example of using "cascading messages"
        // The PongMessage returned by this method would
        // be sent back to the original sender of the PingMessage
        public PongMessage Handle(PingMessage ping)
        {
            Console.WriteLine("Received ping message");
            return new PongMessage();
        }
    }
    // ENDSAMPLE

}