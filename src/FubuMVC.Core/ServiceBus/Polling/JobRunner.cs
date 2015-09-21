using System;
using System.Threading;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class JobRunner<T> where T : IJob
    {
        private readonly T _job;
        private readonly IPollingJobLogger _logger;

        public JobRunner(T job, IPollingJobLogger logger)
        {
            _job = job;
            _logger = logger;
        }

        public void Run(JobRequest<T> request)
        {
            var id = Guid.NewGuid();

            _logger.Starting(id, _job);

            try
            {
                _job.Execute(new CancellationToken());
                _logger.Successful(id, _job);
            }
            catch (Exception e)
            {
                _logger.Failed(id, _job, e);
                throw;
            }
        }
    }
}