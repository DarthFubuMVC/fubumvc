using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class InMemorySchedulePersistence : ISchedulePersistence
    {
        private readonly Cache<string, JobStatusDTO> _statusCache = new Cache<string, JobStatusDTO>(id => {
            var parts = id.Split('/');
            return new JobStatusDTO {JobKey = parts.Last(), NodeName = parts.First()};
        }); 

        private readonly Cache<string, ScheduledRunHistory> _history = new Cache<string, ScheduledRunHistory>(id => new ScheduledRunHistory()); 

        public IEnumerable<JobStatusDTO> FindAllActive(string nodeName)
        {
            return _statusCache.Where(x => x.NodeName == nodeName && x.Status != JobExecutionStatus.Inactive);
        }

        public IEnumerable<JobStatusDTO> FindAll(string nodeName)
        {
            return _statusCache.Where(x => x.NodeName == nodeName);
        }

        public void Persist(IEnumerable<JobStatusDTO> statuses)
        {
            statuses.Each(x => {
                _statusCache[x.Id] = x;
            });
        }

        public void Persist(JobStatusDTO status)
        {
            if (_statusCache.Has(status.Id) && status.LastExecution == null)
            {
                status.LastExecution = _statusCache[status.Id].LastExecution;
            }

            _statusCache[status.Id] = status;
        }

        public JobStatusDTO Find(string nodeName, string jobKey)
        {
            return _statusCache[JobStatusDTO.ToId(nodeName, jobKey)];
        }

        public void RecordHistory(string nodeName, string jobKey, JobExecutionRecord record)
        {
            var id = ScheduledRunHistory.ToId(nodeName, jobKey);
            _history[id].Append(record, 100);

        }

        public IEnumerable<JobExecutionRecord> FindHistory(string nodeName, string jobKey)
        {
            var id = ScheduledRunHistory.ToId(nodeName, jobKey);
            return _history[id].Records;
        }

    }
}