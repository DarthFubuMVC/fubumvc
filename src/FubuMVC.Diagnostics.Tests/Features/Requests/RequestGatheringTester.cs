using System;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Features.Requests.View;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Requests
{
    [TestFixture]
    public class RequestGatheringTester
    {
        private get_Id_handler _handler;
        private DebugReport _report;


        [SetUp]
        public void setup()
        {
            _handler = new get_Id_handler(null, null);
            _report = new DebugReport();
        }

        [Test]
        public void should_isolate_exception_by_generation()
        {
            _report.StartBehavior(new StubBehavior());
            _report.StartBehavior(new ChildBehavior());
            _report.MarkException(new NotImplementedException());
            _report.EndBehavior();
            _report.EndBehavior();

            _handler
                .Gather(_report)
                .Inner
                .Before
                .OfType<ExceptionReport>()
                .ShouldHaveCount(1);
        }

        public class StubBehavior : IActionBehavior
        {
            public void Invoke()
            {
            }

            public void InvokePartial()
            {
            }
        }

        public class ChildBehavior : StubBehavior
        {
        }
    }
}