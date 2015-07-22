using System;
using System.Threading;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Katana;
using FubuMVC.RavenDb.ServiceBus;
using FubuPersistence.RavenDb;
using Raven.Client;
using StructureMap;

namespace ScheduledJobHarness
{
    public class MonitoredNodeGroup : FubuRegistry, IDisposable
    {
        private readonly Cache<string, MonitoredNode> _nodes = new Cache<string, MonitoredNode>();

        // TODO -- replace w/ RavenDb later
        private readonly ISubscriptionPersistence _subscriptions;
        private readonly ISchedulePersistence _schedules;
        private readonly int _port;
        private readonly FubuRuntime _runtime;
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private readonly Container container;
        private readonly IDocumentStore _store;

        public MonitoredNodeGroup()
        {
            _port = PortFinder.FindPort(5500);

            AlterSettings<KatanaSettings>(_ =>
            {
                _.AutoHostingEnabled = true;
                _.Port = _port;
            });

            Services(_ =>
            {
                _.ReplaceService<ISchedulePersistence, RavenDbSchedulePersistence>();
                _.ReplaceService<ISubscriptionPersistence, RavenDbSubscriptionPersistence>();
            });


            ReplaceSettings(RavenDbSettings.InMemory());

            Import<MonitoredTransportRegistry>();

            container = new Container(_ => _.ForSingletonOf<MonitoredNodeGroup>().Use(this));
            this.StructureMap(container);

            _runtime = FubuApplication.For(this).Bootstrap();

            _runtime.Factory.Get<ChannelGraph>().Name = "Monitoring";
            _subscriptions = _runtime.Factory.Get<ISubscriptionPersistence>();
            _schedules = _runtime.Factory.Get<ISchedulePersistence>();
            _store = _runtime.Factory.Get<IDocumentStore>();
        }

        public class MonitoredTransportRegistry : FubuTransportRegistry
        {
            public MonitoredTransportRegistry()
            {
                EnableInMemoryTransport();
            }
        }

        public System.Collections.Generic.IEnumerable<MonitoredNode> Nodes()
        {
            return _nodes;
        }

        public void Shutdown()
        {
            _reset.Set();
        }

        public int Port
        {
            get { return _port; }
        }

        public void Add(string nodeId, Uri incoming)
        {
            var node = new MonitoredNode(nodeId, incoming, _store);
            _nodes[nodeId] = node;
        }

        public MonitoredNode NodeFor(string id)
        {
            return _nodes[id];
        }

        public void Startup()
        {
            _nodes.Each(node => node.Startup(_subscriptions, _schedules));
            var jobs = _nodes.First.Jobs;
            container.Configure(_ => _.For<ScheduledJobGraph>().Use(jobs));
        }

        public void Dispose()
        {
            _nodes.Each(x => x.SafeDispose());
            _runtime.Dispose();
        }

        public void ShutdownNode(string node)
        {
            _nodes[node].Shutdown();
            _nodes.Remove(node);
        }

        public void WaitForShutdown()
        {
            _reset.WaitOne();
        }
    }

    public class TaskState
    {
        public Uri Task { get; set; }
        public string Node { get; set; }
    }
}