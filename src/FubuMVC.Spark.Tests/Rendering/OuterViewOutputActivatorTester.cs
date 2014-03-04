using System.IO;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class OuterViewOutputActivatorTester : InteractionContext<OuterViewOutputActivator>
    {
        private NestedOutput _nestedOutput;
        private ViewOutput _viewOutput;
        private IFubuSparkView _view;
        protected override void beforeEach()
        {
            _viewOutput = MockFor<ViewOutput>();
            _view = MockFor<IFubuSparkView>();
            _view.Stub(x => x.Output).PropertyBehavior();
            _nestedOutput = new NestedOutput();
            Services.Inject(_nestedOutput);
        }

        [Test]
        public void applies_if_nestedoutput_is_not_active()
        {
            ClassUnderTest.Applies(_view).ShouldBeTrue();
            _nestedOutput.SetWriter(() => new StringWriter());
            ClassUnderTest.Applies(_view).ShouldBeFalse();
        }

        [Test]
        public void output_is_viewoutput_instance()
        {
            ClassUnderTest.Modify(_view);
            _view.Output.ShouldEqual(_viewOutput);
        }
    }
}