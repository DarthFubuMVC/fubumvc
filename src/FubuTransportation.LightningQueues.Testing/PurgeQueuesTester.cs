using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Polling;
using FubuTransportation.Testing.ScenarioSupport;
using LightningQueues;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class PurgeQueuesJob_Registration_Tester
    {
        [Test]
        public void PurgeQueuesJob_is_registered()
        {
            using (var runtime = FubuTransport.For<TestRegistry>().StructureMap().Bootstrap())
            {
                runtime.Factory.Get<IPollingJobs>().Any(x => x is PollingJob<PurgeQueuesJob, LightningQueueSettings>)
                    .ShouldBeTrue();
            }
        }

        private class TestRegistry : FubuTransportRegistry<TestSettings>
        {
            public TestRegistry()
            {
                Handlers.DisableDefaultHandlerSource();
                Channel(x => x.Incoming).ReadIncoming().AcceptsMessagesInAssemblyContainingType<OneMessage>();
            }
        }

        private class TestSettings
        {
            public TestSettings()
            {
                Incoming = new Uri("lq.tcp://localhost:2050/test");
            }
            public Uri Incoming { get; set; }
        }
    }

    [TestFixture]
    public class PurgeQueuesTester : InteractionContext<PurgeQueuesJob>
    {
        [Test]
        public void purges_all_queue_managers()
        {
            var queueManagers = Services.CreateMockArrayFor<IQueueManager>(3);
            var persistenQueues = MockFor<IPersistentQueues>();
            persistenQueues.Expect(x => x.AllQueueManagers).Return(queueManagers);

            ClassUnderTest.Execute(new CancellationToken(false));

            queueManagers.Each(x =>
                x.AssertWasCalled(qm => qm.PurgeOldData()));
        }
    }
}