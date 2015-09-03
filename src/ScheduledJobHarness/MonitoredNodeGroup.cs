using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.ServiceBus;
using Raven.Client;
using StructureMap;

namespace ScheduledJobHarness
{
    public class MonitoredNodeGroup : FubuRegistry, IDisposable
    {
        private readonly Cache<string, MonitoredNode> _nodes = new Cache<string, MonitoredNode>();

        // TODO -- replace w/ RavenDb later
        //private readonly ISubscriptionPersistence _subscriptions;
        //private readonly ISchedulePersistence _schedules;
        //private readonly FubuRuntime _runtime;
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        //private readonly Container container;
        //private readonly IDocumentStore _store;

        public MonitoredNodeGroup()
        {
            Port = PortFinder.FindPort(5500);

            throw new Exception("Change this below");

            /*
            AlterSettings<KatanaSettings>(_ =>
            {
                _.AutoHostingEnabled = true;
                _.Port = _port;
            });
             

            Services.ReplaceService<ISchedulePersistence, RavenDbSchedulePersistence>();
            Services.ReplaceService<ISubscriptionPersistence, RavenDbSubscriptionPersistence>();


            ReplaceSettings(RavenDbSettings.InMemory());

            ServiceBus.Configure(x =>
            {
                x.Enabled = true;
                x.EnableInMemoryTransport = true;
            });

            container = new Container(_ => _.ForSingletonOf<MonitoredNodeGroup>().Use(this));
            this.StructureMap(container);

            _runtime = ToRuntime();

            _runtime.Get<ChannelGraph>().Name = "Monitoring";
            _subscriptions = _runtime.Get<ISubscriptionPersistence>();
            _schedules = _runtime.Get<ISchedulePersistence>();
            _store = _runtime.Get<IDocumentStore>();
             * */
        }

        public IEnumerable<MonitoredNode> Nodes()
        {
            return _nodes;
        }

        public void Shutdown()
        {
            _reset.Set();
        }

        public void Add(string nodeId, Uri incoming)
        {
            //var node = new MonitoredNode(nodeId, incoming, _store);
            //_nodes[nodeId] = node;
        }

        public MonitoredNode NodeFor(string id)
        {
            return _nodes[id];
        }

        public void Startup()
        {
            //_nodes.Each(node => node.Startup(_subscriptions, _schedules));
            //var jobs = _nodes.First.Jobs;
            //container.Configure(_ => _.For<ScheduledJobGraph>().Use(jobs));
        }

        public void Dispose()
        {
            _nodes.Each(x => x.SafeDispose());
            //_runtime.Dispose();
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