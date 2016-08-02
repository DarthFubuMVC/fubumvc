using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.LightningQueues;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Monitoring
{
    public class MonitoredNode : FubuTransportRegistry<MonitoringSettings>, IDisposable
    {
        public static readonly Random Random = new Random(100);

        public const string HealthyAndFunctional = "Healthy and Functional";
        public const string TimesOutOnStartupOrHealthCheck = "Times out on startup or health check";
        public const string ThrowsExceptionOnStartupOrHealthCheck = "Throws exception on startup or health check";
        public const string IsInactive = "Is inactive";

        private readonly IList<Uri> _initialTasks = new List<Uri>();

        private readonly Cache<string, FakePersistentTaskSource> _sources =
            new Cache<string, FakePersistentTaskSource>(scheme => new FakePersistentTaskSource(scheme));

        public static readonly InMemorySubscriptionPersistence SubscriptionPersistence = new InMemorySubscriptionPersistence();

        public MonitoredNode(string nodeId, Uri incoming, PersistentTaskMessageListener listener)
        {
            Mode = "Testing";

            AlterSettings<MonitoringSettings>(x => x.Incoming = incoming);
            NodeName = "Monitoring";
            NodeId = nodeId;

            Id = nodeId;

            Channel(x => x.Incoming)
                .DeliveryFastWithoutGuarantee()
                .ReadIncoming()
                .UseAsControlChannel();

            ScheduledJob.ActivatedOnStartup(false);

            Services.AddService<ILogListener>(listener);

            AlterSettings<HealthMonitoringSettings>(_ =>
            {
                _.TakeOwnershipMessageTimeout = 10.Seconds();
                _.HealthCheckMessageTimeout = 1.Seconds();
                _.DeactivationMessageTimeout = 3.Seconds();
                _.TaskAvailabilityCheckTimeout = 5.Seconds();
            });
        }

        public FakePersistentTask TaskFor(string uriString)
        {
            return TaskFor(uriString.ToUri());
        }

        public FakePersistentTask TaskFor(Uri uri)
        {
            return _sources[uri.Scheme][uri];
        }

        public string Id { get; }

        public void Startup(bool monitoringEnabled)
        {
            AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);

            _sources.Each(x => Services.AddService<IPersistentTaskSource>(x));

            Services.For<ISubscriptionPersistence>().Use(SubscriptionPersistence);
            ServiceBus.HealthMonitoring
                .ScheduledExecution(monitoringEnabled
                    ? ScheduledExecution.WaitUntilInterval
                    : ScheduledExecution.Disabled)
                .IntervalSeed(3);

            Runtime = ToRuntime();
            var controller = Runtime.Get<IPersistentTaskController>();

            if (Runtime.Get<ISubscriptionPersistence>() != SubscriptionPersistence)
            {
                throw new Exception("No, wrong persistence somehow!");
            }

            _initialTasks.Each(subject => controller.TakeOwnership(subject).Wait(1.Seconds()));
        }

        void IDisposable.Dispose()
        {
            Runtime.Dispose();
        }

        public void AddTask(Uri subject, IEnumerable<string> preferredNodes)
        {
            TaskFor(subject).PreferredNodes = preferredNodes;
        }

        public Task ActivateTask(Uri subject)
        {
            return Runtime.Get<IPersistentTaskController>().TakeOwnership(subject);
        }

        public void AddInitialTask(Uri subject)
        {
            _initialTasks.Add(subject);
        }

        public IEnumerable<TaskState> AssignedTasks()
        {
            var controller = Runtime.Get<IPersistentTaskController>();
            return controller.ActiveTasks().Select(uri => new TaskState {Node = Id, Task = uri});
        }

        public Task WaitForHealthCheck()
        {
            var jobs = Runtime.Get<IPollingJobs>();
            if (jobs.IsActive<HealthMonitorPollingJob>())
            {
                return jobs.WaitForJobToExecute<HealthMonitorPollingJob>();
            }

            return jobs.ExecuteJob<HealthMonitorPollingJob>();
        }

        public void Shutdown()
        {
            Runtime.Dispose();
        }

        public FubuRuntime Runtime { get; private set; }
    }
}