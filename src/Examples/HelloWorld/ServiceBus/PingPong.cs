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

    public class PingApp : FubuTransportRegistry<HelloWorldSettings>
    {
        public PingApp()
        {
            Channel(x => x.Ponger)
                //.DeliveryFastWithoutGuarantee()
                .AcceptsMessage<PingMessage>();

            Channel(x => x.Pinger)
                //.DeliveryFastWithoutGuarantee()
            .ReadIncoming();
        }
    }

    public class PongApp : FubuTransportRegistry<HelloWorldSettings>
    {
        public PongApp()
        {
            Channel(x => x.Ponger)
                //.DeliveryFastWithoutGuarantee()
                .ReadIncoming();
        }
    }

    public class HelloWorld
    {
        [Fact]
        public void send_and_receive()
        {
            var pinger = FubuRuntime.For<PingApp>();
            var ponger = FubuRuntime.For<PongApp>();

            var bus = pinger.Get<IServiceBus>();

            var pong = bus.Request<PongMessage>(new PingMessage())
                .GetAwaiter().GetResult();

            pong.ShouldNotBe(null);

            pinger.Dispose();
            ponger.Dispose();
        }
    }

    public class StartPingHandler
    {
        public PingMessage Handle(StartPing start)
        {
            Console.WriteLine("Starting ping pong");
            return new PingMessage();
        }

        public void Handle(PongMessage pong)
        {
            Console.WriteLine("Received pong message");
        }
    }

    public class PingHandler
    {
        public PongMessage Handle(PingMessage ping)
        {
            Console.WriteLine("Received ping message");
            return new PongMessage();
        }
    }

    public class PingMessage
    {
    }

    public class StartPing
    {
    }

    public class PongMessage
    {
    }
}