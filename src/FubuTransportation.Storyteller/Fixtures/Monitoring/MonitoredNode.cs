using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuTransportation.Configuration;
using FubuTransportation.LightningQueues;
using FubuTransportation.Monitoring;
using FubuTransportation.Polling;
using FubuTransportation.Subscriptions;

namespace FubuTransportation.Storyteller.Fixtures.Monitoring
{
    public class MonitoredNode : FubuTransportRegistry<MonitoringSettings>, IDisposable
    {
        public static readonly Random Random = new Random(100);

        public const string HealthyAndFunctional = "Healthy and Functional";
        public const string TimesOutOnStartupOrHealthCheck = "Times out on startup or health check";
        public const string ThrowsExceptionOnStartupOrHealthCheck = "Throws exception on startup or health check";
        public const string IsInactive = "Is inactive";

        private readonly string _nodeId;
        private FubuRuntime _runtime;

        private readonly IList<Uri> _initialTasks = new List<Uri>();
        private readonly Cache<string, FakePersistentTaskSource> _sources = new Cache<string, FakePersistentTaskSource>(scheme => new FakePersistentTaskSource(scheme)); 
            
        public MonitoredNode(string nodeId, Uri incoming, PersistentTaskMessageListener listener)
        {
            AlterSettings<MonitoringSettings>(x => x.Incoming = incoming);
            NodeName = "Monitoring";
            NodeId = nodeId;

            _nodeId = nodeId;

            EnableInMemoryTransport(incoming);

            Services(_ => _.AddService<ILogListener>(listener));

            AlterSettings<HealthMonitoringSettings>(_ => {
                _.TakeOwnershipMessageTimeout = 3.Seconds();
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

        public string Id
        {
            get { return _nodeId; }
        }

        public void Startup(bool monitoringEnabled, ISubscriptionPersistence persistence)
        {
            

            AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);

            Services(_ => _sources.Each(_.AddService<IPersistentTaskSource>));
            Services(_ => _.ReplaceService(persistence));
            HealthMonitoring
                .ScheduledExecution(monitoringEnabled ? ScheduledExecution.WaitUntilInterval : ScheduledExecution.Disabled)
                .IntervalSeed(3);

            _runtime = FubuTransport.For(this).StructureMap().Bootstrap();
            var controller = _runtime.Factory.Get<IPersistentTaskController>();

            _initialTasks.Each(subject => {
                controller.TakeOwnership(subject).Wait(1.Seconds());
            });

        }

        void IDisposable.Dispose()
        {
            _runtime.Dispose();
        }

        public void AddTask(Uri subject, IEnumerable<string> preferredNodes)
        {
            TaskFor(subject).PreferredNodes = preferredNodes;
        }

        public Task ActivateTask(Uri subject)
        {
            return _runtime.Factory.Get<IPersistentTaskController>().TakeOwnership(subject);
        }

        public void AddInitialTask(Uri subject)
        {
            _initialTasks.Add(subject);
        }

        public IEnumerable<TaskState> AssignedTasks()
        {
            var controller = _runtime.Factory.Get<IPersistentTaskController>();
            return controller.ActiveTasks().Select(uri => new TaskState {Node = Id, Task = uri});
        }

        public Task WaitForHealthCheck()
        {
            var jobs = _runtime.Factory.Get<IPollingJobs>();
            if (jobs.IsActive<HealthMonitorPollingJob>())
            {
                return jobs.WaitForJobToExecute<HealthMonitorPollingJob>();
            }

            return jobs.ExecuteJob<HealthMonitorPollingJob>();

        }

        public void Shutdown()
        {
            _runtime.Dispose();
        }
    }
}