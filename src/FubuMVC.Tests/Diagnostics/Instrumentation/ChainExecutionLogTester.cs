using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Instrumentation;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class ChainExecutionLogTester
    {
        [Test]
        public void trace_on_parent()
        {
            var log = new StubbedChainExecutionLog();
            log.RequestTime = 100;

            log.Trace("The trace description", () => log.RequestTime = 175);

            var trace = log.Activity.AllSteps().Single().Log.ShouldBeOfType<Trace>();

            trace.Description.ShouldBe("The trace description");
            trace.Duration.ShouldBe(75);
        }

        [Test]
        public void logging_sets_the_request_time_and_activity_at_parent()
        {
            var log = new StubbedChainExecutionLog();
            log.RequestTime = 111;

            log.AddLog(new object());

            log.Activity.Steps.Single().RequestTime.ShouldBe(111);
            log.Activity.Steps.Single().Activity.ShouldBe(log.Activity);
        }

        [Test]
        public void start_and_step_a_subject()
        {
            var subject1 = MockRepository.GenerateMock<ISubject>();
            var subject2 = MockRepository.GenerateMock<ISubject>();

            var log = new StubbedChainExecutionLog();
            log.RequestTime = 111;
            log.StartSubject(subject1);

            log.RequestTime = 222;
            log.StartSubject(subject2);

            log.RequestTime = 333;
            log.FinishSubject();

            log.RequestTime = 444;
            log.FinishSubject();

            log.Activity.Nested[0].Subject.ShouldBe(subject1);
            log.Activity.Nested[0].Start.ShouldBe(111);
            log.Activity.Nested[0].End.ShouldBe(444);

            log.Activity.Nested[0].Nested[0].Subject.ShouldBe(subject2);
            log.Activity.Nested[0].Nested[0].Start.ShouldBe(222);
            log.Activity.Nested[0].Nested[0].End.ShouldBe(333);
        }

        [Test]
        public void deep_activity_logging()
        {
            var subject1 = MockRepository.GenerateMock<ISubject>();
            var subject2 = MockRepository.GenerateMock<ISubject>();

            var x1 = new Object();
            var x2 = new Object();
            var x3 = new Object();
            var x4 = new Object();

            var log = new StubbedChainExecutionLog();
            log.RequestTime = 1;
            log.AddLog(x1);

            log.StartSubject(subject1);
            log.RequestTime = 5;
            log.AddLog(x2);

            log.RequestTime = 10;
            log.StartSubject(subject2);
            log.AddLog(x3);

            log.RequestTime = 15;
            log.FinishSubject();
            log.AddLog(x4);

            var steps = log.Activity.AllSteps().OrderBy(x => x.RequestTime).ToArray();


            steps[0].Activity.Subject.ShouldBe(log);
            steps[1].Activity.Subject.ShouldBe(subject1);

            steps[2].Log.ShouldBe(x3);
            steps[2].Activity.Subject.ShouldBe(subject2);
            steps[3].Activity.Subject.ShouldBe(subject1);
        }


        [Test]
        public void log_exception_once()
        {
            var subject1 = MockRepository.GenerateMock<ISubject>();
            var subject2 = MockRepository.GenerateMock<ISubject>();

            var log = new StubbedChainExecutionLog();
            log.StartSubject(subject1);
            log.StartSubject(subject2);

            var ex = new DivideByZeroException();

            log.LogException(ex);

            log.FinishSubject();
            log.LogException(ex);

            log.FinishSubject();
            log.LogException(ex);



            log.Activity.AllSteps().Select(x => x.Log).OfType<ExceptionReport>()
                .Single().ExceptionText.ShouldBe(ex.ToString());
        }

        [Test]
        public void has_exception()
        {
            var log = new StubbedChainExecutionLog();
            log.HadException.ShouldBeFalse();

            log.LogException(new NotImplementedException());
        
            log.HadException.ShouldBeTrue();
        }

        [Test]
        public void mark_finished()
        {
            var log = new ChainExecutionLog();
            log.MarkFinished(dict => dict.Add("A", 1));

            log.Request["A"].ShouldBe(1);
        }
    }

    public class StubbedChainExecutionLog : ChainExecutionLog
    {
        public StubbedChainExecutionLog() : base()
        {
        }

        public double RequestTime { get; set; }

        protected override double requestTime()
        {
            return RequestTime;
        }
    }
}