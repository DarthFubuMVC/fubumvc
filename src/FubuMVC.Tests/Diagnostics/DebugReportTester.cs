using System;
using System.Linq;
using System.Threading;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Tests.Registration;
using FubuMVC.Tests.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DebugReportTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            report = new DebugReport();
            inner = new FakeBehavior();
        }

        #endregion

        private DebugReport report;
        private FakeBehavior inner;

        [Test]
        public void add_behavior_details_1()
        {
            report.StartBehavior(inner);
            report.StartBehavior(inner);

            var details = new RedirectReport
            {
                Url = "typeof(FakeBehavior)"
            };
            report.AddDetails(details);

            report.StartBehavior(inner);
            report.StartBehavior(inner);

            report.Skip(1).First().ShouldHaveTheSameElementsAs(new BehaviorStart(), details);
        }

        [Test]
        public void add_behavior_details_2()
        {
            report.StartBehavior(inner);
            report.StartBehavior(inner);

            report.EndBehavior();

            var details = new RedirectReport
            {
                Url = "typeof(FakeBehavior)"
            };
            report.AddDetails(details);

            report.First().ShouldHaveTheSameElementsAs(new BehaviorStart(), new BehaviorFinish(), details);
            report.Skip(1).First().Count().ShouldEqual(1);
        }

        [Test]
        public void record_exception_on_a_behavior()
        {
            report.StartBehavior(inner);
            report.StartBehavior(inner);
            report.StartBehavior(inner);
            report.StartBehavior(inner);

            report.MarkException(new NotImplementedException());

            report.EndBehavior();
            report.EndBehavior();
            report.EndBehavior();
            report.MarkException(new ApplicationException("First one is wrong"));

            // The first detail is BehaviorStart, the second is BehaviorFinish
            report.First().Skip(2).First().ShouldBeOfType<ExceptionReport>().Text.ShouldContain("First one is wrong");
            report.Skip(1).First().Count().ShouldEqual(2);
            report.Skip(2).First().Count().ShouldEqual(2);
            report.Last().Skip(1).First().ShouldBeOfType<ExceptionReport>().Text.ShouldContain("NotImplementedException");
        }

        [Test]
        public void record_model_binding()
        {
            report.StartBehavior(inner);
            report.StartBehavior(inner);

            report.StartModelBinding(typeof (BinderTarget));
            var bindingKey = new ModelBindingKey();
            report.AddBindingDetail(bindingKey);
            Thread.Sleep(100);

            var target = new object();

            report.EndModelBinding(target);

            var modelBinding = report.Skip(1).First().Skip(1).First().ShouldBeOfType<ModelBindingReport>();

            modelBinding.StoredObject.ShouldBeTheSameAs(target);
            modelBinding.First().ShouldBeTheSameAs(bindingKey);
            modelBinding.ExecutionTime.ShouldBeGreaterThan(0);
        }

        [Test]
        public void start_and_stop_behaviors()
        {
            report.StartBehavior(inner);
            report.StartBehavior(inner);
            report.StartBehavior(inner);
            report.StartBehavior(inner);

            report.Select(x => x.Description).Count().ShouldEqual(4);
        }
    }
}