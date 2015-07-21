using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public interface ISchedulePersistence
    {
        IEnumerable<JobStatusDTO> FindAll(string nodeName);
        IEnumerable<JobStatusDTO> FindAllActive(string nodeName);
        void Persist(IEnumerable<JobStatusDTO> statuses);
        void Persist(JobStatusDTO status);

        JobStatusDTO Find(string nodeName, string jobKey);

        void RecordHistory(string nodeName, string jobKey, JobExecutionRecord record);
        IEnumerable<JobExecutionRecord> FindHistory(string nodeName, string jobKey);
    }
}