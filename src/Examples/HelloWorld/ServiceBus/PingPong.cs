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

    // SAMPLE: ControlChannelApp
    public class ControlChannelApp : FubuTransportRegistry<AppSettings>
    {
        public ControlChannelApp()
        {
            Channel(x => x.Control)
                .UseAsControlChannel()
                .DeliveryFastWithoutGuarantee();
        }
    }
    // ENDSAMPLE

    // SAMPLE: ListeningApp
    public class ListeningApp : FubuTransportRegistry<HelloWorldSettings>
    {
        public ListeningApp()
        {
            // This directs
            Channel(x => x.Pinger).ReadIncoming();
        }
    }
    // ENDSAMPLE

    // SAMPLE: PersistentMessageChannels
    public class AppSettings
    {
        // This channel handles "fire and forget"
        // control messages
        public Uri Control { get; set; }
            = new Uri("lq.tcp://localhost:2345/control");


        // This channel handles normal business
        // processing messages
        public Uri Transactions { get; set; }
            = new Uri("lq.tcp://localhost:2346/transactions");
    }

    public class BigApp : FubuTransportRegistry<AppSettings>
    {
        public BigApp()
        {
            // Declare that the "Control" channel
            // use the faster, but unsafe transport mechanism
            Channel(x => x.Control)
                .DeliveryFastWithoutGuarantee()
                .UseAsControlChannel();


            Channel(x => x.Transactions)
                // This is the default, but you can
                // still configure it explicitly
                .DeliveryGuaranteed();

        }
    }
    // ENDSAMPLE

    // SAMPLE: sending-messages-for-static-routing
    public class SendingExample
    {
        public async Task SendPingsAndPongs(IServiceBus bus)
        {
            // Publish a message
            bus.Send(new PingMessage());

            // Request/Reply
            var pong = await bus.Request<PongMessage>(new PingMessage());

            // "Delay" Send
            bus.DelaySend(new PingMessage(), TimeSpan.FromDays(1));
        }
    }
    // ENDSAMPLE

    // SAMPLE: StaticRoutingApp
    public class StaticRoutingApp : FubuTransportRegistry<AppSettings>
    {
        public StaticRoutingApp()
        {
            Channel(x => x.Transactions)

                // Explicitly add a single message type
                .AcceptsMessage<PingMessage>()

                // Explicitly add a single message type
                .AcceptsMessage(typeof(PongMessage))

                // Publish any types matching the supplied filter
                // to this channel
                .AcceptsMessages(type => type.Name.EndsWith("Message"))

                // Publish any message type contained in the assembly
                // to this channel, by supplying a type contained
                // within that assembly
                .AcceptsMessagesInAssemblyContainingType<PingMessage>()

                // Publish any message type contained in the named
                // assembly to this channel
                .AcceptsMessagesInAssembly("MyMessageLibrary")

                // Publish any message type contained in the
                // namespace given to this channel
                .AcceptsMessagesInNamespace("MyMessageLibrary")

                // Publish any message type contained in the namespace
                // of the type to this channel
                .AcceptsMessagesInNamespaceContainingType<PingMessage>();
        }
    }
    // ENDSAMPLE




}