using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class ScheduleStatusMonitor : IScheduleStatusMonitor
    {
        private readonly ChannelGraph _channels;
        private readonly ISchedulePersistence _persistence;
        private readonly ILogger _logger;
        private readonly ISystemTime _systemTime;
        private readonly Cache<string, Type> _jobTypes = new Cache<string, Type>(x => null);

        public ScheduleStatusMonitor(ChannelGraph channels, ScheduledJobGraph jobs, ISchedulePersistence persistence,
            ILogger logger, ISystemTime systemTime)
        {
            _channels = channels;
            _persistence = persistence;
            _logger = logger;
            _systemTime = systemTime;

            jobs.Jobs.Each(j => {
                var key = JobStatus.GetKey(j.JobType);
                _jobTypes[key] = j.JobType;
            });
        }


        public void Persist(Action<JobSchedule> scheduling)
        {
            var schedule = _persistence.FindAll(_channels.Name).Select(ToStatus)
                .Where(x => x.JobType != null).ToSchedule();

            scheduling(schedule);

            _persistence.Persist(schedule.Select(x => x.ToDTO(_channels.Name)).ToArray());
        }

        public JobStatus ToStatus(JobStatusDTO dto)
        {
            return new JobStatus(_jobTypes[dto.JobKey])
            {
                Status = dto.Status,
                LastExecution = dto.LastExecution,
                NextTime = dto.NextTime
            };
        }

        private void modifyStatus<T>(Action<JobStatusDTO> change, string failureMessage)
        {
            var jobKey = JobStatus.GetKey(typeof (T));
            try
            {
                var status = _persistence.Find(_channels.Name, jobKey);
                if (status == null)
                {
                    _logger.Info(() => "Unable to find a persisted record of Job {0} on channel {1}, creating a new one".ToFormat(jobKey, _channels.Name));
                    status = new JobStatusDTO {NodeName = _channels.Name, JobKey = jobKey};
                }

                status.Started = null;

                var current = status.LastExecution;

                change(status);

                _persistence.Persist(status);

                if (status.LastExecution != null && !ReferenceEquals(current, status.LastExecution))
                {
                    _persistence.RecordHistory(_channels.Name, jobKey, status.LastExecution);
                }
            }
            catch (Exception e)
            {
                var id = JobStatusDTO.ToId(_channels.Name, jobKey);
                _logger.Error(id, failureMessage, e);
            }
        }

        public void MarkScheduled<T>(DateTimeOffset nextTime)
        {
            modifyStatus<T>(_ => {
                _.Status = JobExecutionStatus.Scheduled;
                _.NextTime = nextTime;
            }, "Trying to mark a scheduled job as scheduled");
        }

        public void MarkExecuting<T>()
        {
            modifyStatus<T>(_ => {
                _.Started = _systemTime.UtcNow();
                _.Status = JobExecutionStatus.Executing;
                _.Executor = _channels.NodeId;
            }, "Trying to mark a scheduled job as executing");
        }

        public void MarkCompletion<T>(JobExecutionRecord record)
        {
            record.Executor = _channels.NodeId;

            modifyStatus<T>(_ => {
                _.Status = record.Success ? JobExecutionStatus.Completed : JobExecutionStatus.Failed;
                _.LastExecution = record;
                _.Executor = null;
            }, "Trying to mark a scheduled job as completed");
        }

        

        public IJobRunTracker TrackJob<T>(int attempts, T job) where T : IJob
        {
            _logger.InfoMessage(() => new ScheduledJobStarted(job));

            return new JobRunTracker<T>(_channels.NodeId, this, job, attempts);
        }

        public class JobRunTracker<T> : IJobRunTracker where T : IJob
        {
            private readonly string _nodeId;
            private readonly ScheduleStatusMonitor _parent;
            private readonly T _job;
            private readonly int _attempts;
            private readonly Stopwatch _stopwatch = new Stopwatch();

            public JobRunTracker(string nodeId, ScheduleStatusMonitor parent, T job, int attempts)
            {
                _nodeId = nodeId;
                _parent = parent;
                _job = job;
                _attempts = attempts;
                _stopwatch.Start();
            }

            private JobExecutionRecord toExecutionRecord()
            {
                _stopwatch.Stop();

                return new JobExecutionRecord
                {
                    Executor = _nodeId,
                    Attempts = _attempts,
                    Duration = _stopwatch.ElapsedMilliseconds,
                    Finished = _parent._systemTime.UtcNow()
                };
            }

            public void Success(DateTimeOffset nextTime)
            {
                var record = toExecutionRecord();
                record.Success = true;

                _parent._logger.InfoMessage(() => new ScheduledJobSucceeded(_job));

                _parent.modifyStatus<T>(_ =>
                {
                    _.Status = JobExecutionStatus.Completed;
                    _.LastExecution = record;
                    _.NextTime = nextTime;
                    _.Executor = null;
                }, "Trying to mark a scheduled job as completed");
            }

            public void Failure(Exception ex)
            {
                var record = toExecutionRecord();
                record.Success = false;
                record.ReadException(ex);

                _parent._logger.Error("Scheduled job {0} failed".ToFormat(_job), ex);
                _parent._logger.InfoMessage(() => new ScheduledJobFailed(_job, ex));

                _parent.MarkCompletion<T>(record);
            }

            public DateTimeOffset Now()
            {
                return _parent._systemTime.UtcNow();
            }
        }
    }
}