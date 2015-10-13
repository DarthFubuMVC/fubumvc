using System;
using System.Data;
using System.Timers;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace DiagnosticsHarness
{
    public class HarnessRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public HarnessRegistry()
        {
            //Comment out in order to test with LightningQueues
            //EnableInMemoryTransport();

            Features.Diagnostics.Enable(TraceLevel.Verbose);

            Services.ForSingletonOf<MessagePumper>();
            Services.ForSingletonOf<INumberCache>().Use<NumberCache>();

            // TODO -- publish everything option in the FI?
            Channel(x => x.Channel).ReadIncoming().AcceptsMessages(x => x != typeof(TraceMessage)).DefaultSerializer<XmlMessageSerializer>();

            Policies.Global.Add<ErrorHandlingPolicy>();

            SubscribeLocally().ToSource(x => x.Publisher)
                .ToMessage<NumberMessage>();

            Channel(x => x.Publisher)
                .ReadIncoming()
                .AcceptsMessage<TraceMessage>()
                .DefaultSerializer<XmlMessageSerializer>();

            Polling.RunJob<SampleJob>().ScheduledAtInterval<HarnessSettings>(x => x.SampleJobTime).RunImmediately();
        }
    }

    public class MessagePumper : IDisposable
    {
        private readonly IServiceBus _bus;
        private readonly Timer _timer = new Timer(500);

        public MessagePumper(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Interval = 1000;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            for (var i = 0; i < 25; i++)
            {
                _bus.Send(new TraceMessage());
            }
        }
    }

    public class ErrorHandlingPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain chain)
        {
            chain.MaximumAttempts = 5;
            chain.OnException<TimeoutException>().Retry();
            chain.OnException<DBConcurrencyException>().Retry();
        }
    }
}