using System;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Tests.View.FakeViews.Folder1;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class ActivityTester
    {


        [Test]
        public void mark_end_and_duration()
        {
            var subject = MockRepository.GenerateMock<ISubject>();
            var activity = new Activity(subject, 125);


            activity.MarkEnd(200);

            activity.Duration.ShouldBe(75);
        }

        [Test]
        public void inner_time()
        {
            var subject = MockRepository.GenerateMock<ISubject>();
            var activity = new Activity(subject, 125);

            activity.MarkEnd(500);

            activity.InnerTime.ShouldBe(500 - 125);
        }

        [Test]
        public void inner_time_with_nesteds()
        {
            var subject = MockRepository.GenerateMock<ISubject>();
            var activity = new Activity(subject, 125);

            activity.MarkEnd(500);

            var nested1 = new Activity(subject, 150);
            nested1.MarkEnd(175);
            var nested2 = new Activity(subject, 250);
            nested2.MarkEnd(290);
            activity.Nested.Add(nested1);
            activity.Nested.Add(nested2);

            activity.InnerTime.ShouldBe(500 - 125 - 25 - 40);
        }
    }
}