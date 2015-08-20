using System;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class ActivityTester
    {
        [Test]
        public void builds_a_log()
        {
            var subject = MockRepository.GenerateMock<ISubject>();
            var activity = new Activity(subject, 125);

            var log = new Object();

            activity.AppendLog(126, log);

            var step = activity.Steps.Single();

            step.Id.ShouldNotBe(Guid.Empty);
            step.Activity.ShouldBe(activity);
            step.Log.ShouldBe(log);
        }

        [Test]
        public void all_steps_with_deep_activity_structure()
        {
            var subject1 = MockRepository.GenerateMock<ISubject>();
            var subject2 = MockRepository.GenerateMock<ISubject>();
            var subject3 = MockRepository.GenerateMock<ISubject>();
            var subject4 = MockRepository.GenerateMock<ISubject>();

            var activity1 = new Activity(subject1, 0);
            var activity2 = new Activity(subject2, 100);
            var activity3 = new Activity(subject3, 200);
            var activity4 = new Activity(subject4, 300);

            var log1 = new object();
            var log2 = new object();
            var log3 = new object();
            var log4 = new object();
            var log5 = new object();
            var log6 = new object();
            var log7 = new object();
        
            activity1.Nested.Add(activity2);
            activity2.Nested.Add(activity3);
            activity2.Nested.Add(activity4);

            activity1.AppendLog(1, log1);
            activity1.AppendLog(2, log2);
            activity2.AppendLog(101, log3);
            activity2.AppendLog(105, log4);
            activity3.AppendLog(205, log5);
            activity3.AppendLog(208, log6);
            activity4.AppendLog(308, log7);

            activity1.AllSteps().OrderBy(x => x.RequestTime)
                .Select(x => x.Log)
                .ShouldHaveTheSameElementsAs(log1, log2, log3, log4, log5, log6, log7);
        }

        [Test]
        public void mark_end_and_duration()
        {
            var subject = MockRepository.GenerateMock<ISubject>();
            var activity = new Activity(subject, 125);


            activity.MarkEnd(200);

            activity.Duration.ShouldBe(75);
        }
    }
}