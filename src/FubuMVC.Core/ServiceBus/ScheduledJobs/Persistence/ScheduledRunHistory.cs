using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class ScheduledRunHistory
    {
        public static string ToId(string node, string job)
        {
            return "History-" + JobStatusDTO.ToId(node, job);
        }

        public string JobKey { get; set; }
        public string NodeName { get; set; }

        public string Id
        {
            get
            {
                return ToId(NodeName, JobKey);
            }
        }
        private readonly Queue<JobExecutionRecord> _records = new Queue<JobExecutionRecord>();

        public JobExecutionRecord[] Records
        {
            get
            {
                return _records.ToArray();
            }
            set
            {
                _records.Clear();
                if (value != null)
                {
                    value.Each(x => _records.Enqueue(x));
                }
            }
        }

        public void Append(JobExecutionRecord record, int maxRecords)
        {
            _records.Enqueue(record);
            while (_records.Count > maxRecords)
            {
                _records.Dequeue();
            }
        }


    }
}