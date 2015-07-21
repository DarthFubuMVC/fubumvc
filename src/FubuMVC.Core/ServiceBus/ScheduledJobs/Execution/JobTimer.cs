using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using Timer = System.Timers.Timer;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Execution
{
    public interface IJobTimer
    {
        void Schedule(Type type, DateTimeOffset time, Action action);
        void ClearAll();
        IEnumerable<ITimedExecution> Status();

        ITimedExecution StatusFor(Type jobType);

        DateTimeOffset Now();
    }

    public interface ITimerCallback
    {
        void Reschedule(DateTimeOffset next);
        void Complete();
        DateTimeOffset Now();
    }

    public class JobTimer : IJobTimer
    {
        private readonly ISystemTime _systemTime;
        private readonly ILogger _logger;
        private readonly Cache<Type, TimedExecution> _executions = new Cache<Type, TimedExecution>();

        public JobTimer(ISystemTime systemTime, ILogger logger)
        {
            _systemTime = systemTime;
            _logger = logger;
        }

        public void Schedule(Type type, DateTimeOffset time, Action action)
        {
            removeExisting(type);

            createNewTimedExecution(type, time, action);
        }

        private void createNewTimedExecution(Type type, DateTimeOffset time, Action action)
        {
            var interval = time.Subtract(_systemTime.UtcNow()).TotalMilliseconds;
            if (interval < 0) interval = 0;


            _executions[type] = new TimedExecution(_logger, type, time, interval, action);
        }

        private void removeExisting(Type type)
        {
            if (_executions.Has(type))
            {
                var execution = _executions[type];
                execution.SafeDispose();
                _executions.Remove(type);
            }
        }

        public void ClearAll()
        {
            _executions.ToArray().Each(x => x.SafeDispose());
            _executions.ClearAll();
        }

        public IEnumerable<ITimedExecution> Status()
        {
            return _executions;
        }

        public ITimedExecution StatusFor(Type jobType)
        {
            return _executions.FirstOrDefault(x => x.Type == jobType);
        }

        public DateTimeOffset Now()
        {
            return _systemTime.UtcNow();
        }

        /// <summary>
        /// Only usable for very controlled testing!!!
        /// </summary>
        /// <param name="jobType"></param>
        /// <param name="timeout"></param>
        public void WaitForJob(Type jobType, TimeSpan timeout)
        {
            _executions[jobType].WaitForCompletion(timeout);
        }
    }

    public interface ITimedExecution
    {
        JobExecutionStatus Status { get; }
        Type Type { get; }
        DateTimeOffset ExpectedTime { get; }
    }

    public class TimedExecution : IDisposable, ITimedExecution
    {
        private readonly Type _type;
        private readonly DateTimeOffset _expectedTime;
        private readonly Timer _timer;
        private readonly ManualResetEvent _finished = new ManualResetEvent(false);

        public TimedExecution(ILogger logger, Type type, DateTimeOffset expectedTime, double millisecondsToWait,
            Action action)
        {
            if (millisecondsToWait <= 0) millisecondsToWait = 1;

            Status = JobExecutionStatus.Scheduled;
            _type = type;
            _expectedTime = expectedTime;

            _timer = new Timer {AutoReset = false, Interval = millisecondsToWait};
            _timer.Elapsed += (sender, args) => {
                Status = JobExecutionStatus.Executing;
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    logger.Error("Trying to execute Scheduled job " + type.GetFullName(), e);
                }

                _finished.Set();
            };

            _timer.Enabled = true;
        }

        public void WaitForCompletion(TimeSpan timeout)
        {
            _finished.WaitOne(timeout);
        }

        public JobExecutionStatus Status { get; private set; }

        public Type Type
        {
            get { return _type; }
        }

        public DateTimeOffset ExpectedTime
        {
            get { return _expectedTime; }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}