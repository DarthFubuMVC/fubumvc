using System;

namespace FubuMVC.Tests.ServiceBus.Docs.GettingStarted
{
    // SAMPLE: GettingStartedHandlers
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
    // ENDSAMPLE
}