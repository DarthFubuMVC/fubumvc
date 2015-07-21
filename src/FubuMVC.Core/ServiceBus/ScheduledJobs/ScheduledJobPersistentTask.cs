using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs
{
    public class ScheduledJobPersistentTask : IPersistentTaskSource, IPersistentTask
    {
        private readonly IScheduledJobController _controller;
        public static readonly Uri Uri = "scheduled://jobs".ToUri();

        public ScheduledJobPersistentTask(IScheduledJobController controller)
        {
            _controller = controller;
        }

        public string Protocol
        {
            get
            {
                return Uri.Scheme;
            }
        }

        public IEnumerable<Uri> PermanentTasks()
        {
            yield return Uri;
        }

        public IPersistentTask CreateTask(Uri uri)
        {
            if (uri != Uri) throw new ArgumentOutOfRangeException("uri","Only '{0}' is allowed in this method".ToFormat(uri));

            return this;
        }

        public Uri Subject
        {
            get
            {
                return Uri;
            }
        }

        public void AssertAvailable()
        {
            _controller.PerformHealthChecks();
        }

        public void Activate()
        {
            _controller.Activate();
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        public bool IsActive
        {
            get
            {
                return _controller.IsActive();
            }
        }

        public Task<ITransportPeer> SelectOwner(IEnumerable<ITransportPeer> peers)
        {
            var ordered = peers.OrderBy(x => x.ControlChannel.ToString());
            var completion = new OrderedAssignment(Uri, ordered);

            return completion.SelectOwner();
        }
    }
}