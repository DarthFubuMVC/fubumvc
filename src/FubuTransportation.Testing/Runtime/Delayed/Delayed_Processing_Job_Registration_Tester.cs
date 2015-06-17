using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTransportation.Configuration;
using FubuTransportation.Polling;
using FubuTransportation.Runtime.Delayed;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using StructureMap;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.Runtime.Delayed
{
    [TestFixture]
    public class Delayed_Processing_Job_Registration_Tester
    {
        private FubuRuntime runtime;

        [Test]
        public void the_delayed_processing_polling_job_is_registered()
        {
            FubuTransport.SetupForInMemoryTesting();

            runtime = FubuTransport.For<DelayedRegistry>().StructureMap(new Container())
                           .Bootstrap();

            runtime.Factory.Get<IPollingJobs>().Any(x => x is PollingJob<DelayedEnvelopeProcessor, TransportSettings>)
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
            Services(x => x.ReplaceService<ISystemTime>(new SettableClock()));
            Handlers.Include<SimpleHandler<OneMessage>>();
            Channel(x => x.Downstream).ReadIncoming().AcceptsMessagesInAssemblyContainingType<OneMessage>();
        }
    }
}