using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class PersistentTaskController : ITransportPeer, IPersistentTasks, IPersistentTaskController
    {
        private readonly ChannelGraph _graph;
        private readonly ILogger _logger;
        private readonly ITaskMonitoringSource _factory;

        private readonly ConcurrentCache<string, IPersistentTaskSource> _sources
            = new ConcurrentCache<string, IPersistentTaskSource>();

        private readonly ConcurrentCache<Uri, IPersistentTaskAgent> _agents =
            new ConcurrentCache<Uri, IPersistentTaskAgent>();

        public override string ToString()
        {
            return $"PersistentTaskController: {NodeId}";
        }

        private readonly Uri[] _permanentTasks;


        public PersistentTaskController(ChannelGraph graph, ILogger logger, ITaskMonitoringSource factory, IList<IPersistentTaskSource> sources)
        {
            _graph = graph;
            _logger = logger;
            _factory = factory;
            sources.Each(x => _sources[x.Protocol] = x);

            _agents.OnMissing = uri => {
                var persistentTask = FindTask(uri);
                return persistentTask == null ? null : _factory.BuildAgentFor(persistentTask);
            };

            _permanentTasks = sources.SelectMany(x => x.PermanentTasks()).ToArray();
        }

        public async Task<HealthStatus> CheckStatus(Uri subject)
        {
            var agent = _agents[subject];

            return agent == null ? HealthStatus.Unknown : await checkStatus(agent).ConfigureAwait(false);
        }

        private static async Task<HealthStatus> checkStatus(IPersistentTaskAgent agent)
        {
            return agent.IsActive 
                ? await agent.AssertAvailable().ConfigureAwait(false) 
                : HealthStatus.Inactive;
        }


        public IPersistentTask FindTask(Uri subject)
        {
            if (!_sources.Has(subject.Scheme)) return null;

            var source = _sources[subject.Scheme];

            return source?.CreateTask(subject);
        }

        public IPersistentTaskAgent FindAgent(Uri subject)
        {
            return _agents[subject];
        }

        public async Task<bool> Deactivate(Uri subject)
        {
            var agent = _agents[subject];
            if (agent == null)
            {
                _logger.Info("Task '{0}' is not recognized by this node".ToFormat(subject));

                return false;
            }

            return await agent.Deactivate().ConfigureAwait(false);
        }


        public async Task EnsureTasksHaveOwnership()
        {
            var healthChecks = AllPeers().Select(async x =>
            {
                var status = await x.CheckStatusOfOwnedTasks().ConfigureAwait(false);
                return new { Peer = x, Response = status };
            }).ToArray();

            var checks = await Task.WhenAll(healthChecks).ConfigureAwait(false);


            var planner = new TaskHealthAssignmentPlanner(_permanentTasks);
            foreach (var check in checks)
            {
                planner.Add(check.Peer, check.Response);
            }


            var corrections = planner.ToCorrectionTasks(this);

            await Task.WhenAll(corrections).ConfigureAwait(false);

            _logger.Info(() => "Finished running task health monitoring on node " + NodeId);
        }


        public IEnumerable<ITransportPeer> AllPeers()
        {
            yield return this;
            foreach (var peer in _factory.BuildPeers())
            {
                yield return peer;
            }
        }

        public async Task<OwnershipStatus> TakeOwnership(Uri subject)
        {
            _logger.InfoMessage(() => new TryingToAssignOwnership(subject, NodeId));

            var agent = _agents[subject];
            if (agent == null)
            {
                return OwnershipStatus.UnknownSubject;
            }

            if (agent.IsActive)
            {
                return OwnershipStatus.AlreadyOwned;
            }


            return await agent.Activate().ConfigureAwait(false);
        }

        public async Task<TaskHealthResponse> CheckStatusOfOwnedTasks()
        {
            var subjects = CurrentlyOwnedSubjects().ToArray();

            if (!subjects.Any())
            {
                return TaskHealthResponse.Empty();
            }

            var checks = subjects
                .Select(async subject =>
                {
                    var status = await CheckStatus(subject).ConfigureAwait(false);
                    
                    return new PersistentTaskStatus(subject, status);
                })
                .ToArray();

            var statusList = await Task.WhenAll(checks).ConfigureAwait(false);

            return new TaskHealthResponse
            {
                Tasks = statusList.ToArray()
            };
        }

        public void RemoveOwnershipFromNode(IEnumerable<Uri> subjects)
        {
            _factory.RemoveOwnershipFromThisNode(subjects);
        }

        public IEnumerable<Uri> ActiveTasks()
        {
            return _agents.Where(x => x.IsActive).Select(x => x.Subject).ToArray();
        }

        public IEnumerable<Uri> PermanentTasks()
        {
            return _permanentTasks;
        }

        public IEnumerable<Uri> CurrentlyOwnedSubjects()
        {
            var activeTasks = _agents.Where(x => x.IsActive).Select(x => x.Subject);
            return
                _factory.LocallyOwnedTasksAccordingToPersistence().Union(activeTasks).ToArray();
        }

        public string NodeId => _graph.NodeId;

        public async Task Reassign(Uri subject, IList<ITransportPeer> availablePeers, IList<ITransportPeer> deactivations)
        {
            Debug.WriteLine($"Reassigning task {subject}, available peers {availablePeers.Select(x => x.NodeId).Join(", ")}");

            await Task.WhenAll(deactivations.Select(x => x.Deactivate(subject))).ConfigureAwait(false);

            _logger.InfoMessage(() => new ReassigningTask(subject, deactivations));

            var agent = _agents[subject];
            if (agent == null)
            {
                _logger.InfoMessage(() => new UnknownTask(subject, "Trying to reassign a persistent task"));
                return;
            }

            try
            {
                var owner = await agent.AssignOwner(availablePeers).ConfigureAwait(false);
                if (owner == null)
                {
                    _logger.InfoMessage(() => new UnableToAssignOwnership(subject));
                }
            }
            catch (Exception e)
            {
                _logger.Error(subject, "Failed while trying to assign ownership", e);
            }
        }

        string ITransportPeer.MachineName => System.Environment.MachineName;

        // TODO -- think this should be explicitly set later
        public Uri ControlChannel => _graph.ReplyUriList().FirstOrDefault();
    }
}