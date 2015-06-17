using System;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Polling;
using FubuTransportation.Runtime.Routing;
using FubuTransportation.ScheduledJobs;
using FubuTransportation.ScheduledJobs.Configuration;
using FubuTransportation.ScheduledJobs.Execution;
using FubuTransportation.ScheduledJobs.Persistence;
using NUnit.Framework;
using StructureMap;

namespace FubuTransportation.Testing.ScheduledJobs
{
    [TestFixture]
    public class ScheduledJobIntegrationTester
    {
        private Container container;
        private FubuRuntime theRuntime;

        [TestFixtureSetUp]
        public void SetUp()
        {
            FubuTransport.AllQueuesInMemory = true;

            AJob.Reset();
            BJob.Reset();
            CJob.Reset();

            theRuntime = FubuTransport.For<ScheduledJobRegistry>()
                .StructureMap()
                .Bootstrap();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        [Test]
        public void the_scheduled_job_chains_are_present()
        {
            var graph = theRuntime.Factory.Get<BehaviorGraph>();
            var chains = graph.Behaviors.Where(x => x.InputType() != null && x.InputType().Closes(typeof (ExecuteScheduledJob<>)));

            chains.ShouldHaveCount(3);
        }

        [Test]
        public void the_chains_for_rescheduling_a_job_are_present()
        {
            var graph = theRuntime.Factory.Get<BehaviorGraph>();
            var chains = graph.Behaviors.Where(x => x.InputType() != null && x.InputType().Closes(typeof(RescheduleRequest<>)));

            chains.ShouldHaveCount(3);
        }

        [Test]
        public void registration_of_scheduled_jobs_can_capture_channel_names()
        {
            var graph = theRuntime.Factory.Get<ScheduledJobGraph>();
            graph.DefaultChannel.Name.ShouldEqual("Downstream");
            graph.FindJob(typeof (AJob)).Channel.Name.ShouldEqual("Upstream");
        }

        [Test]
        public void explicitly_register_a_routing_rule_for_a_scheduled_job()
        {
            var graph = theRuntime.Factory.Get<ChannelGraph>();
            graph.ChannelFor<BusSettings>(x => x.Upstream).ShouldExecuteJob<AJob>();
        }

        [Test]
        public void jobs_are_registered_in_the_container()
        {
            theRuntime.Factory.Get<IScheduledJob<AJob>>().ShouldNotBeNull();
            theRuntime.Factory.Get<IScheduledJob<BJob>>().ShouldNotBeNull();
            theRuntime.Factory.Get<IScheduledJob<CJob>>().ShouldNotBeNull();
        }

        [Test]
        public void use_the_default_channel_for_scheduled_jobs_if_none_is_explicitly_set()
        {
            var graph = theRuntime.Factory.Get<ChannelGraph>();
            graph.ChannelFor<BusSettings>(x => x.Downstream)
                .ShouldExecuteJob<BJob>()
                .ShouldExecuteJob<CJob>();
        }

        [Test]
        public void can_override_the_default_timeout()
        {
            var graph = theRuntime.Factory.Get<ScheduledJobGraph>();
            graph.FindJob(typeof (BJob))
                .Timeout.ShouldEqual(11.Minutes());
        }

    }

    public static class ChannelGraphExtensions
    {
        public static ChannelNode ShouldExecuteJob<T>(this ChannelNode node) where T : IJob
        {
            node.Rules.OfType<ScheduledJobRoutingRule<T>>().ShouldHaveCount(1);
            return node;
        }
    }

    public class ScheduledJobRegistry : FubuTransportRegistry<BusSettings>
    {
        public ScheduledJobRegistry()
        {
            EnableInMemoryTransport();

            ScheduledJob.DefaultJobChannel(x => x.Downstream);
            ScheduledJob.RunJob<AJob>().ScheduledBy<DummyScheduleRule>().Channel(x => x.Upstream);
            ScheduledJob.RunJob<BJob>().ScheduledBy<DummyScheduleRule>().Timeout(11.Minutes());
            ScheduledJob.RunJob<CJob>().ScheduledBy<DummyScheduleRule>();
        }
    }

    public class AJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation) { ++Executed; }
        public static void Reset() { Executed = 0; }
    }

    public class BJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation) { ++Executed; }
        public static void Reset() { Executed = 0; }
    }

    public class CJob : IJob
    {
        public static int Executed = 0;

        public void Execute(CancellationToken cancellation) { ++Executed; }
        public static void Reset() { Executed = 0; }
    }

    public class DummyScheduleRule : IScheduleRule
    {
        private readonly DateTimeOffset _nextTime;

        public DummyScheduleRule()
        {
            _nextTime = DateTimeOffset.UtcNow.AddDays(1);
        }

        public DummyScheduleRule(DateTimeOffset nextTime)
        {
            _nextTime = nextTime;
        }

        public DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution)
        {
            return _nextTime;
        }
    }
}
