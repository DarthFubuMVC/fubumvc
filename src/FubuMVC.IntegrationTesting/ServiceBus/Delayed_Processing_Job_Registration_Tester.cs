using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Tests.ServiceBus;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.IntegrationTesting.ServiceBus
{
    [TestFixture]
    public class Delayed_Processing_Job_Registration_Tester
    {
        [Test]
        public void the_delayed_processing_polling_job_is_registered()
        {
            using (var runtime = FubuRuntime.For<DelayedRegistry>())
            {
                runtime.Behaviors.Chains.Where(x => x.InputType() == typeof(JobRequest<DelayedEnvelopeProcessor>))
                    .Each(x => Debug.WriteLine(x.Title()));

                runtime.Get<IPollingJobs>().Any(x => x is PollingJob<DelayedEnvelopeProcessor, TransportSettings>)
                    .ShouldBeTrue();
            }
        }
    }

    public class DelayedRegistry : FubuTransportRegistry<BusSettings>
    {
        public DelayedRegistry()
        {
            AlterSettings<BusSettings>(x => x.Downstream = new Uri("lq.tcp://localhost:2207/delayed"));

            Services.ReplaceService<ISystemTime>(new SettableClock());
            Handlers.Include<SimpleHandler<OneMessage>>();
            Channel(x => x.Downstream).ReadIncoming().AcceptsMessagesInAssemblyContainingType<OneMessage>();
        }
    }
}