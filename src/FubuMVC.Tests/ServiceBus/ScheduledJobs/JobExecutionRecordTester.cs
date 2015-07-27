using System;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    [TestFixture]
    public class JobExecutionRecordTester
    {
        [Test]
        public void read_normal_exception()
        {
            var record = new JobExecutionRecord();
            var ex = new DivideByZeroException("Only Chuck Norris can do that");

            record.ReadException(ex);

            record.ExceptionText.ShouldBe(ex.ToString());
        }

        [Test]
        public void read_aggregate_exception()
        {
            var ex1 = new DivideByZeroException("Only Chuck Norris can do that");
            var ex2 = new RankException("You're last!");
            var ex3 = new InvalidTimeZoneException("You are in the wrong place!");

            var ex = new AggregateException(ex1, ex2, ex3);

            var record = new JobExecutionRecord();
            record.ReadException(ex);

            record.ExceptionText.ShouldNotBe(ex.ToString());
            record.ExceptionText.ShouldContain(ex1.ToString());
            record.ExceptionText.ShouldContain(ex2.ToString());
            record.ExceptionText.ShouldContain(ex3.ToString());

            record.ExceptionText.ShouldContain(JobExecutionRecord.ExceptionSeparator);
        }
    }
}