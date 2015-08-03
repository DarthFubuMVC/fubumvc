using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.RavenDb.ServiceBus;
using Raven.Client;

namespace ScheduledJobHarness
{
    public class ErrorHandlingPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            //when retrying, do so a maximum of 5 times
            handlerChain.MaximumAttempts = 3;

            //retry now
            handlerChain.OnException<DivideByZeroException>()
                .RetryLater(5.Seconds());
        }
    }

    public class MonitoredNode : FubuTransportRegistry<MonitoringSettings>, IDisposable
    {
        public static readonly Random Random = new Random(100);

        public const string HealthyAndFunctional = "Healthy and Functional";
        public const string TimesOutOnStartupOrHealthCheck = "Times out on startup or health check";
        public const string ThrowsExceptionOnStartupOrHealthCheck = "Throws exception on startup or health check";
        public const string IsInactive = "Is inactive";

        private readonly string _nodeId;
        private FubuRuntime _runtime;

        public MonitoredNode(string nodeId, Uri incoming, IDocumentStore store)
        {
            Policies.Local.Add<ErrorHandlingPolicy>();

            AlterSettings<MonitoringSettings>(x => x.Incoming = "memory://jobs".ToUri());

            Channel(x => x.Incoming).ReadIncoming();

            NodeName = "Monitoring";
            NodeId = nodeId;

            _nodeId = nodeId;

            ServiceBus.EnableInMemoryTransport(incoming);


            Services.ReplaceService<ISubscriptionPersistence, RavenDbSubscriptionPersistence>();
            Services.ReplaceService<ISchedulePersistence, RavenDbSchedulePersistence>();
            Services.ReplaceService(store);

            Services.AddService<ILogListener, ScheduledJobListener>();


            ScheduledJob.DefaultJobChannel(x => x.Incoming);
            ScheduledJob.RunJob<AlwaysGoodJob>().ScheduledBy<TenMinuteIncrements>();
            ScheduledJob.RunJob<FailsFirstTwoTimes>().ScheduledBy<SevenMinuteIncrements>();
            ScheduledJob.RunJob<FailsSometimes>().ScheduledBy<FiveMinuteIncrements>();
            ScheduledJob.RunJob<TimesOutSometimes>().ScheduledBy<ThreeMinuteIncrements>().Timeout(10.Seconds());
        }

        public string Id
        {
            get { return _nodeId; }
        }

        public ScheduledJobGraph Jobs
        {
            get { return _runtime.Get<ScheduledJobGraph>(); }
        }

        public void Startup(ISubscriptionPersistence subscriptions, ISchedulePersistence schedules)
        {
            Services.ReplaceService(subscriptions);
            Services.ReplaceService(schedules);

            _runtime = ToRuntime();
        }

        void IDisposable.Dispose()
        {
            _runtime.Dispose();
        }

        public void Shutdown()
        {
            _runtime.Dispose();
        }
    }
}