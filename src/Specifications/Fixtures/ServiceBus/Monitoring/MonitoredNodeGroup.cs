using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Subscriptions;
using Serenity.ServiceBus;
using StoryTeller;

namespace Specifications.Fixtures.ServiceBus.Monitoring
{
    public class MonitoredNodeGroup : IEnumerable<MonitoredNode>, IDisposable
    {
        private readonly Cache<string, MonitoredNode> _nodes = new Cache<string, MonitoredNode>();
        private readonly System.Collections.Generic.IList<Action<MonitoredNode>> _configurations =
            new System.Collections.Generic.List<Action<MonitoredNode>>();

        private readonly PersistentTaskMessageListener _listener = new PersistentTaskMessageListener();

        public void Add(string nodeId, Uri incoming)
        {
            var node = new MonitoredNode(nodeId, incoming, _listener);
            _nodes[nodeId] = node;
        }

        public void AddTask(Uri subject, string initialNode,
            System.Collections.Generic.IEnumerable<string> preferredNodes)
        {
            _configurations.Add(node => node.AddTask(subject, preferredNodes));

            if (!initialNode.EqualsIgnoreCase("none"))
            {
                _nodes[initialNode].AddInitialTask(subject);
            }
        }

        public bool MonitoringEnabled { get; set; }

        public System.Collections.Generic.IEnumerable<PersistentTaskMessage> LoggedEvents()
        {
            return _listener.LoggedEvents().Where(x => x.Subject.Scheme != "scheduled").ToArray();
        }

        public MonitoredNode NodeFor(string id)
        {
            return _nodes[id];
        }

        public void Startup()
        {
            _nodes.Each(node =>
            {
                _configurations.Each(x => x(node));
                node.Startup(MonitoringEnabled);
            });
        }

        public void Dispose()
        {
            _nodes.Each(x => x.SafeDispose());
        }

        public void SetTaskState(Uri subject, string node, string state)
        {
            var task = _nodes[node].TaskFor(subject);
            task.SetState(state, MonitoredNode.SubscriptionPersistence, node);
        }

        public System.Collections.Generic.IEnumerable<TaskState> AssignedTasks()
        {
            return _nodes.SelectMany(x => x.AssignedTasks()).Where(x => x.Task.Scheme != "scheduled");
        }

        public System.Collections.Generic.IEnumerable<TaskState> PersistedTasks()
        {
            return
                MonitoredNode.SubscriptionPersistence.AllNodes()
                    .SelectMany(
                        node => { return node.OwnedTasks.Select(x => new TaskState {Node = node.Id, Task = x}); })
                    .Where(x => x.Task.Scheme != "scheduled");
        }

        public void WaitForAllHealthChecks()
        {
            var tasks =
                _nodes.Select(
                    x =>
                    {
                        return
                            Task.Delay(MonitoredNode.Random.Next(25, 200))
                                .ContinueWith(t => x.WaitForHealthCheck())
                                .Unwrap();
                    });

            Task.WaitAll(tasks.ToArray(), 15.Seconds());
        }

        public void ShutdownNode(string node)
        {
            _nodes[node].Shutdown();
            _nodes.Remove(node);
        }

        public System.Collections.Generic.IEnumerable<TransportNode> GetPersistedNodes()
        {
            return MonitoredNode.SubscriptionPersistence.NodesForGroup("Monitoring");
        }

        public void WaitForHealthChecksOn(string node)
        {
            _nodes[node].WaitForHealthCheck().Wait(120.Seconds());
        }

        public void AddLogs(ISpecContext context)
        {
            _nodes.Each(node =>
            {
                var session = node.Runtime.Get<IMessagingSession>();

                var provider = new MessageContextualInfoProvider(session) {Title = node.Id, ShortTitle = node.Id};

                context.Reporting.Log(node.Id, provider.ToHtml(), node.Id);
            });
        }

        public IEnumerator<MonitoredNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TaskState
    {
        public Uri Task { get; set; }
        public string Node { get; set; }
    }
}