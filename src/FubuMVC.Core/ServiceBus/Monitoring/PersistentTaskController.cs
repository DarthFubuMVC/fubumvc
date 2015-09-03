using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface IPersistentTasks
    {
        IPersistentTask FindTask(Uri subject);
        IPersistentTaskAgent FindAgent(Uri subject);
        string NodeId { get; }

        Task Reassign(Uri subject, IEnumerable<ITransportPeer> availablePeers, IEnumerable<ITransportPeer> deactivations);
    }

    public interface IPersistentTaskController
    {
        Task<HealthStatus> CheckStatus(Uri subject);
        Task<bool> Deactivate(Uri subject);
        Task EnsureTasksHaveOwnership();
        Task<OwnershipStatus> TakeOwnership(Uri subject);
        Task<TaskHealthResponse> CheckStatusOfOwnedTasks();
        IEnumerable<Uri> ActiveTasks();
        IEnumerable<Uri> PermanentTasks();
    }

    public class PersistentTaskController : ITransportPeer, IPersistentTasks, IPersistentTaskController
    {
        private readonly ChannelGraph _graph;
        private readonly ILogger _logger;
        private readonly ITaskMonitoringSource _factory;

        private readonly ConcurrentCache<string, IPersistentTaskSource> _sources
            = new ConcurrentCache<string, IPersistentTaskSource>();

        private readonly ConcurrentCache<Uri, IPersistentTaskAgent> _agents =
            new ConcurrentCache<Uri, IPersistentTaskAgent>();


        private readonly Uri[] _permanentTasks;


        public PersistentTaskController(ChannelGraph graph, ILogger logger, ITaskMonitoringSource factory,
            IEnumerable<IPersistentTaskSource> sources)
        {
            _graph = graph;
            _logger = logger;
            _factory = factory;
            sources.Each(x => _sources[x.Protocol] = x);

            _agents.OnMissing = uri => {
                var persistentTask = FindTask(uri);
                if (persistentTask == null) return null;

                return _factory.BuildAgentFor(persistentTask);
            };

            _permanentTasks = sources.SelectMany(x => x.PermanentTasks()).ToArray();
        }

        public Task<HealthStatus> CheckStatus(Uri subject)
        {
            var agent = _agents[subject];

            if (agent == null)
            {
                return HealthStatus.Unknown.ToCompletionTask();
            }

            return checkStatus(agent);
        }

        private Task<HealthStatus> checkStatus(IPersistentTaskAgent agent)
        {
            return agent.IsActive ? agent.AssertAvailable() : HealthStatus.Inactive.ToCompletionTask();
        }


        public IPersistentTask FindTask(Uri subject)
        {
            if (!_sources.Has(subject.Scheme)) return null;

            var source = _sources[subject.Scheme];
            if (source == null) return null;

            return source.CreateTask(subject);
        }

        public IPersistentTaskAgent FindAgent(Uri subject)
        {
            return _agents[subject];
        }

        public Task<bool> Deactivate(Uri subject)
        {
            var agent = _agents[subject];
            if (agent == null)
            {
                _logger.Info("Task '{0}' is not recognized by this node".ToFormat(subject));

                return false.ToCompletionTask();
            }

            return agent.Deactivate();
        }


        public Task EnsureTasksHaveOwnership()
        {
            var healthChecks = allPeers().Select(x => x.CheckStatusOfOwnedTasks().ContinueWith(_ => {
                return new {Peer = x, Response = _.Result};
            }));

            return Task.WhenAll(healthChecks).ContinueWith(checks => {
                var planner = new TaskHealthAssignmentPlanner(_permanentTasks);
                checks.Result.Each(_ => planner.Add(_.Peer, _.Response));

                var corrections = planner.ToCorrectionTasks(this);

                return Task.WhenAll(corrections).ContinueWith(_ => {

                    _logger.Info(() => "Finished running task health monitoring on node " + NodeId);

                }, TaskContinuationOptions.AttachedToParent);
            }, TaskContinuationOptions.AttachedToParent);
        }


        private IEnumerable<ITransportPeer> allPeers()
        {
            yield return this;
            foreach (var peer in _factory.BuildPeers())
            {
                yield return peer;
            }
        }

        public Task<OwnershipStatus> TakeOwnership(Uri subject)
        {
            _logger.InfoMessage(() => new TryingToAssignOwnership(subject, NodeId));

            var agent = _agents[subject];
            if (agent == null)
            {
                return OwnershipStatus.UnknownSubject.ToCompletionTask();
            }

            if (agent.IsActive)
            {
                return OwnershipStatus.AlreadyOwned.ToCompletionTask();
            }


            return agent.Activate();
        }

        public Task<TaskHealthResponse> CheckStatusOfOwnedTasks()
        {
            var subjects = CurrentlyOwnedSubjects();

            if (!subjects.Any())
            {
                return TaskHealthResponse.Empty().ToCompletionTask();
            }

            var checks = subjects
                .Select(subject => CheckStatus(subject).ContinueWith(t => new PersistentTaskStatus(subject, t.Result)))
                .ToArray();

            return Task.Factory.ContinueWhenAll(checks, tasks => new TaskHealthResponse
            {
                Tasks = tasks.Select(x => x.Result).ToArray()
            });
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

        public string NodeId
        {
            get { return _graph.NodeId; }
        }

        public Task Reassign(Uri subject, IEnumerable<ITransportPeer> availablePeers, IEnumerable<ITransportPeer> deactivations)
        {
            deactivations = deactivations.ToArray();
            return Task.WhenAll(deactivations.Select(x => x.Deactivate(subject)))
                .ContinueWith(_ => {
                    _logger.InfoMessage(() => new ReassigningTask(subject, deactivations));

                    var agent = _agents[subject];
                    if (agent == null)
                    {
                        _logger.InfoMessage(() => new UnknownTask(subject, "Trying to reassign a persistent task"));
                        return true.ToCompletionTask();
                    }
                    else
                    {
                        return agent.AssignOwner(availablePeers).ContinueWith(t => {
                            if (t.IsCompleted && t.Result == null)
                            {
                                _logger.InfoMessage(() => new UnableToAssignOwnership(subject));
                            }

                            if (t.IsFaulted)
                            {
                                _logger.Error(subject, "Failed while trying to assign ownership", t.Exception);
                            }
                        });
                    }
                }, TaskContinuationOptions.AttachedToParent);
        }

        string ITransportPeer.MachineName
        {
            get { return System.Environment.MachineName; }
        }

        // TODO -- think this should be explicitly set later
        public Uri ControlChannel
        {
            get { return _graph.ReplyUriList().FirstOrDefault(); }
        }
    }
}