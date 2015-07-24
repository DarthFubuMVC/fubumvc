using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using Raven.Client;

namespace FubuMVC.RavenDb.ServiceBus
{
    public class RavenDbSchedulePersistence : ISchedulePersistence
    {
        private readonly ITransaction _transaction;
        private readonly IDocumentStore _store;
        private readonly int _maxHistory;

        public RavenDbSchedulePersistence(ITransaction transaction, IDocumentStore store, ScheduledJobGraph jobs)
        {
            _transaction = transaction;
            _store = store;
            _maxHistory = jobs.MaxJobExecutionRecordsToKeepInHistory;
        }

        public IEnumerable<JobStatusDTO> FindAll(string nodeName)
        {
            using (var session = _store.OpenSession())
            {
                return session
                    .Query<JobStatusDTO>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.NodeName == nodeName)
                    .ToArray();
            }
        }

        public IEnumerable<JobStatusDTO> FindAllActive(string nodeName)
        {
            using (var session = _store.OpenSession())
            {
                return session
                    .Query<JobStatusDTO>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.NodeName == nodeName)
                    .Where(x => x.Status != JobExecutionStatus.Inactive)
                    .ToArray();
            }
        }

        public void Persist(IEnumerable<JobStatusDTO> statuses)
        {
            _transaction.Execute<IDocumentSession>(session => { statuses.Each(x => session.Store(x)); });
        }

        public void Persist(JobStatusDTO status)
        {
            _transaction.Execute<IDocumentSession>(session => { session.Store(status); });
        }

        public JobStatusDTO Find(string nodeName, string jobKey)
        {
            var id = JobStatusDTO.ToId(nodeName, jobKey);
            using (var session = _store.OpenSession())
            {
                return session.Load<JobStatusDTO>(id);
            }
        }


        public void RecordHistory(string nodeName, string jobKey, JobExecutionRecord record)
        {
            var id = ScheduledRunHistory.ToId(nodeName, jobKey);

            _transaction.Execute<IDocumentSession>(session => {
                var history = session.Load<ScheduledRunHistory>(id)
                    ?? new ScheduledRunHistory{JobKey = jobKey, NodeName = nodeName};

                history.Append(record, _maxHistory);
                session.Store(history);
            });
        }

        public IEnumerable<JobExecutionRecord> FindHistory(string nodeName, string jobKey)
        {
            var id = ScheduledRunHistory.ToId(nodeName, jobKey);
            using (var session = _store.OpenSession())
            {
                var history = session.Load<ScheduledRunHistory>(id);
                return history == null ? new JobExecutionRecord[0] : history.Records.ToArray();
            }
        }
    }
}