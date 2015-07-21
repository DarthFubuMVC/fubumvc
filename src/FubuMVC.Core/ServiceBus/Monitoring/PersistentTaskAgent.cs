using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    // Error handling and logging is handled in PersistentTaskController
    public interface IPersistentTaskAgent
    {
        Uri Subject { get; }
        bool IsActive { get; }
        Task<HealthStatus> AssertAvailable();
        Task<OwnershipStatus> Activate();
        Task<bool> Deactivate();
        Task<ITransportPeer> AssignOwner(IEnumerable<ITransportPeer> peers);
    }

    public class PersistentTaskAgent : IPersistentTaskAgent
    {
        private readonly IPersistentTask _task;
        private readonly HealthMonitoringSettings _settings;
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _repository;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public PersistentTaskAgent(IPersistentTask task, HealthMonitoringSettings settings, ILogger logger, ISubscriptionRepository repository)
        {
            _task = task;
            _settings = settings;
            _logger = logger;
            _repository = repository;
        }

        public Uri Subject
        {
            get { return _task.Subject; }
        }

        public Task<HealthStatus> AssertAvailable()
        {
            return Task.Factory.StartNew(() => {
                var status =
                        TimeoutRunner.Run(_settings.TaskAvailabilityCheckTimeout, () => _task.AssertAvailable(),
                            ex => {
                                _logger.Error(Subject, "Availability test failed for " + Subject, ex);
                                _logger.InfoMessage(() => new TaskAvailabilityFailed(Subject)
                                {
                                    ExceptionText = ex.ToString(),
                                    ExceptionType = ex.GetType().Name
                                });
                            });

                switch (status)
                {
                    case Completion.Exception:
                        
                        return HealthStatus.Error;

                    case Completion.Success:
                        return HealthStatus.Active;

                    case Completion.Timedout:
                        _logger.InfoMessage(() => new TaskAvailabilityFailed(Subject){ExceptionType = "Timedout"});
                        return HealthStatus.Timedout;

                    default:
                        throw new ArgumentOutOfRangeException("status", "Status " + status + " should not be possible here");
                }
            }, TaskCreationOptions.AttachedToParent);
        }

        public Task<OwnershipStatus> Activate()
        {
            return Task.Factory.StartNew(
                () => activate(),
                TaskCreationOptions.AttachedToParent
                );
        }

        private OwnershipStatus activate()
        {
            Action activation = () => _lock.Write(() => _task.Activate());

            var status = TimeoutRunner.Run(_settings.TaskActivationTimeout, activation, ex => {
                _logger.Error(Subject, "Failed to take ownership of task " + Subject, ex);
                _logger.InfoMessage(() => new TaskActivationFailure(Subject));
            });

            switch (status)
            {
                 case Completion.Success:
                    _logger.InfoMessage(() => new TookOwnershipOfPersistentTask(Subject));
                    _repository.AddOwnershipToThisNode(Subject);
                    return OwnershipStatus.OwnershipActivated;
            
                case Completion.Timedout:
                    _logger.InfoMessage(() => new TaskActivationTimeoutFailure(Subject));
                    return OwnershipStatus.TimedOut;

                default:
                    return OwnershipStatus.Exception;
            }
        }

        public Task<bool> Deactivate()
        {
            return Task.Factory.StartNew(
                () => deactivate(),
                TaskCreationOptions.AttachedToParent);
        }

        private bool deactivate()
        {
            try
            {
                _lock.Write(() => _task.Deactivate());
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(Subject, "Failed to stop task " + Subject, ex);
                _logger.InfoMessage(() => new FailedToStopTask(Subject));

                return false;
            }
            finally
            {
                _repository.RemoveOwnershipFromThisNode(Subject);
            }
        }

        public bool IsActive
        {
            get
            {
                return _lock.Read(() => _task.IsActive);
            }
        }

        public Task<ITransportPeer> AssignOwner(IEnumerable<ITransportPeer> peers)
        {
            return _task.SelectOwner(peers);
        }
    }
}