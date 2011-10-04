using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Features.Requests;
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
        private BehaviorDetailsModel _model;

        [SetUp]
        public void setup()
        {
            _handler = new get_Id_handler(null, null);
            _report = new DebugReport();
            _model = getModel();
        }

        [Test]
        public void should_build_outer_most_behavior()
        {
            _model
                .BehaviorType
                .ShouldEqual(typeof (StubBehavior));

            _model
                .Before
                .ShouldHaveCount(2); // start behavior, start model binding (end model binding doesn't register)

            _model
                .After
                .ShouldHaveCount(1); // end behavior

            _model
                .Inner
                .BehaviorType
                .ShouldEqual(typeof (ChildBehavior));
        }

        [Test]
        public void should_build_nested_behavior()
        {
            var target = _model.Inner;
            target
                .BehaviorType
                .ShouldEqual(typeof(ChildBehavior));

            target
                .Before
                .ShouldHaveCount(2);

            target
                .Inner
                .BehaviorType
                .ShouldEqual(typeof (ChildBehavior2));

            target
                .After
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_build_second_nested_behavior()
        {
            var target = _model.Inner.Inner;
            target
                .BehaviorType
                .ShouldEqual(typeof (ChildBehavior2));

            target
                .Before
                .ShouldHaveCount(2);

            target
                .Inner
                .ShouldBeNull();

            target
                .After
                .ShouldHaveCount(1);
        }

        private BehaviorDetailsModel getModel()
        {
            // e.g., ActionCall -> WebFormView -> Partial WebFormView
            _report.StartBehavior(new StubBehavior());
            _report.StartModelBinding(typeof(StubModel));
            _report.EndModelBinding(new StubModel());
            _report.StartBehavior(new ChildBehavior());
            _report.StartModelBinding(typeof(StubModel));
            _report.EndModelBinding(new StubModel());
            _report.StartBehavior(new ChildBehavior2());
            _report.StartModelBinding(typeof(StubModel));
            _report.EndModelBinding(new StubModel());
            _report.EndBehavior();
            _report.EndBehavior();
            _report.EndBehavior();

            return _handler.Gather(_report);
        }

        public class StubModel {}

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

        public class ChildBehavior2 : StubBehavior
        {
        }
    }
}