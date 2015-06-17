using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using FubuCore.Reflection;

namespace FubuTransportation.Polling
{
    public class PollingJob<TJob, TSettings> : DescribesItself, IPollingJob where TJob : IJob
    {
        private readonly IServiceBus _bus;
        private readonly IPollingJobLogger _logger;
        private readonly TSettings _settings;
        private readonly ITimer _timer;
        private readonly Expression<Func<TSettings, double>> _intervalSource;
        private readonly ScheduledExecution _scheduledExecution;
        private readonly PollingJobLatch _latch;
        private readonly Func<TSettings, double> _intervalFunc; 
        private readonly IList<TaskCompletionSource<object>> _waiters = new List<TaskCompletionSource<object>>(); 
        private readonly object _waiterLock = new object();

        public PollingJob(IServiceBus bus, IPollingJobLogger logger, TSettings settings,
            PollingJobDefinition definition, PollingJobLatch latch)
        {
            _bus = bus;
            _logger = logger;
            _settings = settings;
            _timer = new DefaultTimer();
            _intervalSource = (Expression<Func<TSettings, double>>)definition.IntervalSource;
            _scheduledExecution = definition.ScheduledExecution;
            _latch = latch;

            _intervalFunc = _intervalSource.Compile();
        }

        public ScheduledExecution ScheduledExecution
        {
            get { return _scheduledExecution; }
        }

        public Task WaitForJobToExecute()
        {
            lock (_waiterLock)
            {
                var source = new TaskCompletionSource<object>();
                _waiters.Add(source);
                return source.Task;
            }
        }

        public void Describe(Description description)
        {
            description.Title = "Polling Job for " + typeof(TJob).Name;
            typeof(TJob).ForAttribute<DescriptionAttribute>(att => description.ShortDescription = att.Description);
            description.Properties["Interval"] = _intervalFunc(_settings) + " ms";
            description.Properties["Config"] = _intervalSource.ToString();
            description.Properties["Scheduled Execution"] = _scheduledExecution.ToString();
        }

        public bool IsRunning()
        {
            return _timer.Enabled;
        }

        public void Start()
        {
            if (_scheduledExecution == ScheduledExecution.RunImmediately)
            {
                RunNow();
            }

            _timer.Start(RunNow, _intervalFunc(_settings));
        }

        public void RunNow()
        {
            if (_latch.Latched) return;

            TaskCompletionSource<object>[] waiters = null;

            lock (_waiterLock)
            {
                waiters = _waiters.ToArray();
                 _waiters.Clear();
            }

            try
            {
                _bus.Consume(new JobRequest<TJob>());
                waiters.Each(x => x.SetResult(new object()));
            }
            catch (Exception e)
            {
                _waiters.Each(x => x.SetException(e));

                // VERY unhappy with the code below, but I cannot determine
                // why the latching doesn't work cleanly in the NUnit console runner
                if (_latch.Latched || e.Message.Contains("Could not find an Instance named")) return;

                if (!_latch.Latched)
                {
                    _logger.FailedToSchedule(typeof(TJob), e);
                }
            }
            finally
            {
                _timer.Interval = _intervalFunc(_settings);
            }
        }

        public void Stop()
        {
            _logger.Stopping(typeof(TJob));
            _timer.Stop();
        }

        public Type JobType
        {
            get
            {
                return typeof (TJob);
            }
        }

        public void Dispose()
        {
            Stop();
            _timer.Dispose();
        }

    }
}