using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    [TestFixture]
    public class JobTimerIntegratedTester
    {
        [Test]
        public void TimeExecution_actually_runs()
        {
            bool executed = false;

            var execution = new TimedExecution(new RecordingLogger(), GetType(), DateTime.Today, 15, () => {
                executed = true;


            });

            execution.WaitForCompletion(30.Seconds());

            executed.ShouldBeTrue();
        }


        [Test, Explicit("It's too slow to be in CI")]
        public void smoke_test_of_JobTimer_execution()
        {
            var now = DateTimeOffset.UtcNow;

            var ajobTime = now.AddSeconds(50);
            var bjobTime = now.AddSeconds(25);
            var cjobTime = now.AddSeconds(10);

            var timer = new JobTimer(SystemTime.Default(), new RecordingLogger());
            timer.Schedule(typeof(AJob), ajobTime, () => Debug.WriteLine("AJob expected at {0} and was at {1}", ajobTime, DateTime.UtcNow));
            timer.Schedule(typeof(BJob), bjobTime, () => Debug.WriteLine("BJob expected at {0} and was at {1}", bjobTime, DateTime.UtcNow));
            timer.Schedule(typeof(CJob), cjobTime, () => Debug.WriteLine("CJob expected at {0} and was at {1}", cjobTime, DateTime.UtcNow));


            timer.WaitForJob(typeof (AJob), 100.Seconds());
            timer.WaitForJob(typeof (BJob), 100.Seconds());
            timer.WaitForJob(typeof (CJob), 100.Seconds());
        }


    }
}