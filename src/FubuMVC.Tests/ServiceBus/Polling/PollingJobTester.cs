using System;
using System.Threading;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [System.ComponentModel.Description("A polling job just for testing purposes")]
    public class APollingJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
            
        }
    }

    
    public class PollingJobTester : InteractionContext<PollingJob<APollingJob, PollingJobSettings>>
    {
        protected override void beforeEach()
        {
            var definition = PollingJobChain.For<APollingJob, PollingJobSettings>(x => x.Polling);
            Services.Inject(definition);
        }

        [Fact]
        public void run_now_successfully()
        {
            ClassUnderTest.RunNow();

            MockFor<IServiceBus>().AssertWasCalled(x => x.Consume(new JobRequest<APollingJob>()), x => x.IgnoreArguments());
        }

        [Fact]
        public void run_now_with_a_failure()
        {
            var ex = new NotImplementedException();
            MockFor<IServiceBus>().Expect(x => x.Consume(new JobRequest<APollingJob>()))
                                  .IgnoreArguments()
                                  .Throw(ex);

            ClassUnderTest.RunNow();

            MockFor<IPollingJobLogger>().AssertWasCalled(x => x.FailedToSchedule(typeof(APollingJob), ex));
        }

        [Fact]
        public void smoke_test_describe()
        {
            var description = Description.For(ClassUnderTest);
            description.Title.ShouldBe("Polling Job for APollingJob");
            description.ShortDescription.ShouldBe("A polling job just for testing purposes");
            description.Properties["Interval"].ShouldBe("350 ms");
            description.Properties["Config"].ShouldBe("x => x.Polling");
            description.Properties["Scheduled Execution"].ShouldBe("WaitUntilInterval");
        }
    }

    public class PollingJobSettings : DescribesItself
    {
        public PollingJobSettings()
        {
            Polling = 350;
        }

        public double Polling { get; set; }
        public void Describe(Description description)
        {
            description.Properties[nameof(Polling)] = $"{Polling} ms";
        }
    }
}