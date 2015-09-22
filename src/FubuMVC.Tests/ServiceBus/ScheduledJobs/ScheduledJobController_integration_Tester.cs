using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    [TestFixture, Explicit("This test is necessarily *slow*")]
    public class ScheduledJobController_integration_Tester
    {
        private FubuRuntime runtime;
        private JobHistory history;
        private RewindableClock clock;
        private IScheduledJobController theController;
        private ISchedulePersistence thePersistence;
        private IJobTimer theTimer;
        private DateTimeOffset theStartingTime;
        private DateTimeOffset theEndingTime;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            //FubuTransport.AllQueuesInMemory = true;

            runtime = FubuRuntime.For<TestingJobRegistry>();

            history = runtime.Get<JobHistory>();
            clock = runtime.Get<ISystemTime>().As<RewindableClock>();
            theController = runtime.Get<IScheduledJobController>();
            thePersistence = runtime.Get<ISchedulePersistence>();
            theTimer = runtime.Get<IJobTimer>();

            theController.Deactivate();
            theTimer.ClearAll();
            history.ClearAll();

            theStartingTime = (DateTimeOffset) DateTime.Today.AddHours(8).ToUniversalTime();

            theEndingTime = theStartingTime.AddSeconds(30);

            addTimes<Job1>(3);
            addTimes<Job2>(10);
            addTimes<Job3>(15);

            clock.Reset(theStartingTime.DateTime);

            theController.Activate();

            theController.IsActive().ShouldBeTrue();

            Thread.Sleep(32.Seconds());

            theController.Deactivate();
        }

        [Test]
        public void all_the_expected_jobs_ran_successfully()
        {
            history.AssertAll();
        }

        private void addTimes<T>(int seconds) where T : IJob
        {
            // START HERE AFTER LUNCH
            var next = theStartingTime.AddSeconds(seconds);
            while (next <= theEndingTime)
            {
                history.Expect(typeof (T), next);
                next = next.AddSeconds(seconds);
            }
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            runtime.Dispose();
        }
    }

    public class RewindableClock : ISystemTime
    {
        private TimeSpan _offset = 0.Seconds();

        public void Reset(DateTime utc)
        {
            _offset = utc.Subtract(DateTime.UtcNow);
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow.Add(_offset);
        }

        public LocalTime LocalTime()
        {
            return new LocalTime(UtcNow(), TimeZoneInfo.Local);
        }
    }

    public class TestingJobRegistry : FubuTransportRegistry<BusSettings>
    {
        public TestingJobRegistry()
        {
            ServiceBus.EnableInMemoryTransport();

            Channel(x => x.Upstream).ReadIncoming();

            ScheduledJob.ActivatedOnStartup(false);
            ScheduledJob.DefaultJobChannel(x => x.Upstream);
            ScheduledJob.RunJob<Job1>().ScheduledBy(new AtSecondsAfterTheMinute(3));
            ScheduledJob.RunJob<Job2>().ScheduledBy(new AtSecondsAfterTheMinute(10));
            ScheduledJob.RunJob<Job3>().ScheduledBy(new AtSecondsAfterTheMinute(15));

            Services.ReplaceService(new JobHistory());
            Services.ReplaceService<ISystemTime>(new RewindableClock());
        }
    }

    public class Job1 : IJob
    {
        private readonly JobHistory _history;
        private readonly ISystemTime _systemTime;

        public Job1(JobHistory history, ISystemTime systemTime)
        {
            _history = history;
            _systemTime = systemTime;
        }

        public void Execute(CancellationToken cancellation)
        {
            _history.CaptureActual(GetType(), _systemTime.UtcNow());
            Thread.Sleep(250);
        }
    }

    public class Job2 : Job1
    {
        public Job2(JobHistory history, ISystemTime systemTime) : base(history, systemTime)
        {
        }
    }

    public class Job3 : Job1
    {
        public Job3(JobHistory history, ISystemTime systemTime)
            : base(history, systemTime)
        {
        }
    }

    public class AtSecondsAfterTheMinute : IScheduleRule
    {
        private readonly int _seconds;

        public AtSecondsAfterTheMinute(int seconds)
        {
            _seconds = seconds;
        }

        public DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution)
        {
            var next = currentTime.Subtract(new TimeSpan(0, 0, 0, currentTime.Second, currentTime.Millisecond));

            while (next < currentTime)
            {
                next = next.Add(_seconds.Seconds());
            }

            return next;
        }
    }


    public class JobHistory
    {


        private readonly Cache<Type, Expectation> _expectations = new Cache<Type, Expectation>(t => new Expectation(t));

        public void Expect(Type type, DateTimeOffset expected)
        {
            _expectations[type].Expect(expected.ToUniversalTime());
        }

        public void CaptureActual(Type type, DateTimeOffset time)
        {
            _expectations[type].CaptureActual(time);
        }

        public void AssertAll()
        {
            var list = new List<string>();
            _expectations.Each(x => x.Assert(list));

            if (list.Any())
            {
                var message = list.Join(System.Environment.NewLine);
                Assert.Fail(message);
            }
        }

        public class Expectation
        {
            private readonly Type _type;
            private readonly IList<DateTimeOffset> _expected = new List<DateTimeOffset>();
            private readonly IList<DateTimeOffset> _actual = new List<DateTimeOffset>();

            public Expectation(Type type)
            {
                _type = type;
            }

            public Type Type
            {
                get { return _type; }
            }

            public void Expect(DateTimeOffset expected)
            {
                _expected.Add(expected);
            }

            public void CaptureActual(DateTimeOffset actual)
            {
                _actual.Add(actual);
            }

            public void Assert(IList<string> list)
            {
                if (_actual.Count > _expected.Count)
                {
                    var expectedMessage = _expected.Select(x => x.ToLocalTime().ToString()).Join(", ");
                    var actualMessage = _actual.Select(x => x.ToLocalTime().ToString()).Join(", ");
                    list.Add("More executions of job {0} than expected. Expected {1}, but got {2}".ToFormat(_type.Name,
                        expectedMessage, actualMessage));
                }

                for (var i = 0; i < _expected.Count; i++)
                {
                    if (i >= _actual.Count)
                    {
                        list.Add("Expected execution of job {0} at {1}, but none was recorded".ToFormat(_type.Name,
                            _expected[i].ToLocalTime()));
                    }
                    else
                    {
                        var actual = _actual[i];
                        var expected = _expected[i];

                        var totalMilliseconds = Math.Abs(actual.Subtract(expected).TotalMilliseconds);


                        if (totalMilliseconds > 750)
                        {
                            list.Add(
                                "Expected execution of job {0} at {1}, but was at {2} / {3} ms different".ToFormat(
                                    _type.Name,
                                    expected.ToLocalTime(), actual.ToLocalTime(), totalMilliseconds));
                        }
                    }
                }
            }
        }

        public void ClearAll()
        {
            _expectations.ClearAll();
        }
    }
}