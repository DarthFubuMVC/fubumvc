using System.Linq;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Tests.ServiceBus.ScenarioSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Delayed
{
    [TestFixture]
    public class Delayed_Processing_Job_Registration_Tester
    {
        private FubuRuntime runtime;

        [Test]
        public void the_delayed_processing_polling_job_is_registered()
        {
            FubuTransport.SetupForInMemoryTesting();

            runtime = FubuRuntime.For<DelayedRegistry>()
                           ;

            runtime.Get<IPollingJobs>().Any(x => x is PollingJob<DelayedEnvelopeProcessor, TransportSettings>)
                .ShouldBeTrue();
        }

        [TearDown]
        public void TearDown()
        {
            runtime.Dispose();
            FubuTransport.Reset();
        }
    }

    public class DelayedRegistry : FubuTransportRegistry<BusSettings>
    {
        public DelayedRegistry()
        {
            Services.ReplaceService<ISystemTime>(new SettableClock());
            Handlers.Include<SimpleHandler<OneMessage>>();
            Channel(x => x.Downstream).ReadIncoming().AcceptsMessagesInAssemblyContainingType<OneMessage>();
        }
    }
}