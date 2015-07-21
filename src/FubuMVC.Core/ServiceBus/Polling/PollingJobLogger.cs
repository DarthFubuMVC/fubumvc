using System;
using FubuCore;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobLogger : IPollingJobLogger
    {
        private readonly ILogger _logger;

        public PollingJobLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Stopping(Type jobType)
        {
            _logger.DebugMessage(() => new PollingJobStopped { JobType = jobType });
        }

        public void Starting(Guid id, IJob job)
        {
            _logger.InfoMessage(() => new PollingJobStarted { Description = job.ToString(), JobRun = id });
        }

        public void Successful(Guid id, IJob job)
        {
            _logger.InfoMessage(() => new PollingJobSuccess { Description = job.ToString(), JobRun = id });
        }

        public void Failed(Guid id, IJob job, Exception ex)
        {
            _logger.Error("Job {0}".ToFormat(job), ex);
            _logger.InfoMessage(() => new PollingJobFailed { Description = job.ToString(), Exception = ex, JobRun = id });

        }

        public void FailedToSchedule(Type jobType, Exception exception)
        {
            _logger.Error("Job {0} could not be Scheduled to run".ToFormat(jobType.FullName), exception);
        }
    }
}