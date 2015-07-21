using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskHealthResponse
    {
        public PersistentTaskStatus[] Tasks { get; set; }
        public bool ResponseFailed { get; set; }

        public void AddMissingSubjects(IEnumerable<Uri> subjects)
        {
            var tasks = Tasks ?? new PersistentTaskStatus[0];

            var fills = subjects.Where(x => tasks.All(_ => _.Subject != x))
                .Select(x => new PersistentTaskStatus(x, HealthStatus.Inactive));

            Tasks = tasks.Union(fills).ToArray();
        }

        public static TaskHealthResponse ErrorFor(IEnumerable<Uri> enumerable)
        {
            return new TaskHealthResponse
            {
                Tasks = enumerable.Select(x => new PersistentTaskStatus(x, HealthStatus.Error)).ToArray(),
                ResponseFailed = true
            };
        }

        public static TaskHealthResponse Empty()
        {
            return new TaskHealthResponse
            {
                Tasks = new PersistentTaskStatus[0]
            };
        }

        public override string ToString()
        {
            return "Health Check " + Tasks.Select(x => "{0}: {1}".ToFormat(x.Subject, x.Status)).Join(", ");
        }

        public IEnumerable<Uri> AllSubjects()
        {
            return (Tasks ?? new PersistentTaskStatus[0]).Select(x => x.Subject);
        }
    }
}