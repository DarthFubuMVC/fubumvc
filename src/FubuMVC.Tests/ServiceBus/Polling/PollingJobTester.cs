using System;
using System.Threading;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.TestSupport;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Polling
{
    [System.ComponentModel.Description("A polling job just for testing purposes")]
    public class APollingJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
            
        }
    }

    [TestFixture]
    public class PollingJobTester : InteractionContext<PollingJob<APollingJob, PollingJobSettings>>
    {
        protected override void beforeEach()
        {
            var definition = PollingJobDefinition.For<APollingJob, PollingJobSettings>(x => x.Polling);
            Services.Inject(definition);
        }

        [Test]
        public void run_now_successfully()
        {
            ClassUnderTest.RunNow();

            MockFor<IServiceBus>().AssertWasCalled(x => x.Consume(new JobRequest<APollingJob>()), x => x.IgnoreArguments());
        }

        [Test]
        public void run_now_with_a_failure()
        {
            var ex = new NotImplementedException();
            MockFor<IServiceBus>().Expect(x => x.Consume(new JobRequest<APollingJob>()))
                                  .IgnoreArguments()
                                  .Throw(ex);

            ClassUnderTest.RunNow();

            MockFor<IPollingJobLogger>().AssertWasCalled(x => x.FailedToSchedule(typeof(APollingJob), ex));
        }

        [Test]
        public void smoke_test_describe()
        {
            var description = Description.For(ClassUnderTest);
            description.Title.ShouldEqual("Polling Job for APollingJob");
            description.ShortDescription.ShouldEqual("A polling job just for testing purposes");
            description.Properties["Interval"].ShouldEqual("350 ms");
            description.Properties["Config"].ShouldEqual("x => x.Polling");
            description.Properties["Scheduled Execution"].ShouldEqual("WaitUntilInterval");
        }
    }

    public class PollingJobSettings
    {
        public PollingJobSettings()
        {
            Polling = 350;
        }

        public double Polling { get; set; }
    }
}