using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    [Serializable]
    public abstract class PersistentTaskMessage : LogRecord
    {
        public PersistentTaskMessage(Uri subject)
        {
            Subject = subject;
        }

        public PersistentTaskMessage()
        {
        }

        public Uri Subject { get; set; }
        public string NodeId { get; set; }
        public string Machine { get; set; }

        protected bool Equals(PersistentTaskMessage other)
        {
            return Equals(Subject, other.Subject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PersistentTaskMessage) obj);
        }

        public override int GetHashCode()
        {
            return (Subject != null ? Subject.GetHashCode() : 0);
        }
    }

    public class TaskActivationFailure : PersistentTaskMessage
    {
        public TaskActivationFailure(Uri subject) : base(subject)
        {
        }

        public TaskActivationFailure()
        {
        }
    }

    public class TaskActivationTimeoutFailure : PersistentTaskMessage
    {
        public TaskActivationTimeoutFailure(Uri subject) : base(subject)
        {
        }

        public TaskActivationTimeoutFailure()
        {
        }

        public override string ToString()
        {
            return "Task Activation for {0} timed out on node {1}".ToFormat(Subject, NodeId);
        }
    }

    public class TryingToAssignOwnership : PersistentTaskMessage
    {
        public TryingToAssignOwnership(Uri subject, string toNode) : base(subject)
        {
            ToNode = toNode;
        }

        public TryingToAssignOwnership()
        {
        }

        public string ToNode { get; set; }

        public override string ToString()
        {
            return "Trying to assign ownership of task {0} to node {1} from node {2}"
                .ToFormat(Subject, ToNode, NodeId);
        }
    }

    public class TakeOwnershipRequestReceived : PersistentTaskMessage
    {
        public TakeOwnershipRequestReceived(Uri subject, Uri @from) : base(subject)
        {
            From = @from;
        }

        public TakeOwnershipRequestReceived()
        {
        }

        public Uri From { get; set; }
        public OwnershipStatus Status { get; set; }


        public override string ToString()
        {
            return "Received a request to take ownership of {0} from {1} on node {2} finished with a response of {3}"
                .ToFormat(Subject, From, NodeId, Status);
        }
    }

    public class TookOwnershipOfPersistentTask : PersistentTaskMessage
    {
        public TookOwnershipOfPersistentTask()
        {
        }

        public TookOwnershipOfPersistentTask(Uri subject) : base(subject)
        {
        }

        public override string ToString()
        {
            return "Took ownership of task {0} on node {1}".ToFormat(Subject, NodeId);
        }
    }

    public class FailedToActivatePersistentTask : PersistentTaskMessage
    {
        public FailedToActivatePersistentTask(Uri subject) : base(subject)
        {
        }

        public FailedToActivatePersistentTask()
        {
        }

        public override string ToString()
        {
            return "Failed to activate task {0} on node {1}".ToFormat(Subject, NodeId);
        }
    }

    public class StoppedTask : PersistentTaskMessage
    {
        public StoppedTask(Uri subject) : base(subject)
        {
        }

        public StoppedTask()
        {
        }

        public override string ToString()
        {
            return "Stopping task {0} on node {1}".ToFormat(Subject, NodeId);
        }
    }

    public class FailedToStopTask : PersistentTaskMessage
    {
        public FailedToStopTask(Uri subject) : base(subject)
        {
        }

        public FailedToStopTask()
        {
        }
    }

    public class TaskAvailabilityFailed : PersistentTaskMessage
    {
        public TaskAvailabilityFailed(Uri subject) : base(subject)
        {
        }

        public string ExceptionText { get; set; }

        public string ExceptionType { get; set; }

        public TaskAvailabilityFailed()
        {
        }

        public override string ToString()
        {
            return string.Format("Task availability failed for {0} on node {1} with exception type {2}", Subject, NodeId,
                ExceptionType);
        }
    }

    public class ReassigningTask : PersistentTaskMessage
    {
        public ReassigningTask(Uri subject, IEnumerable<ITransportPeer> deactivations) : base(subject)
        {
            CurrentNodes = deactivations.Select(x => x.NodeId).ToArray();
        }

        public string[] CurrentNodes { get; set; }

        public ReassigningTask()
        {
        }

        public override string ToString()
        {
            if (CurrentNodes.Any())
            {
                return "Re-assigning task {0} from node {1}, was on node(s) {2}"
                    .ToFormat(Subject, NodeId, CurrentNodes.Join(", "));
            }

            return "Assigning task {0} from node {1}".ToFormat(Subject, NodeId);
        }
    }


    public class UnknownTask : PersistentTaskMessage
    {
        public UnknownTask(Uri subject, string context) : base(subject)
        {
            Context = context;
        }

        public string Context { get; set; }

        public UnknownTask()
        {
        }
    }

    public class TaskAssignment : PersistentTaskMessage
    {
        public string AssignedTo { get; set; }

        public TaskAssignment(Uri subject, string assignedTo) : base(subject)
        {
            AssignedTo = assignedTo;
        }

        public TaskAssignment()
        {
        }
    }

    public class UnableToAssignOwnership : PersistentTaskMessage
    {
        public UnableToAssignOwnership(Uri subject) : base(subject)
        {
        }

        public UnableToAssignOwnership()
        {
        }
    }
}