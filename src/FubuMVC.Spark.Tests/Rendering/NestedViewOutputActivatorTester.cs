using System.IO;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class NestedViewOutputActivatorTester : InteractionContext<NestedViewOutputActivator>
    {
        private IFubuSparkView _view;
        private NestedOutput _nestedOutput;
        protected override void beforeEach()
        {
            _view = MockFor<IFubuSparkView>();
            _view.Stub(x => x.Output).PropertyBehavior();
            _nestedOutput = new NestedOutput();
            Services.Inject(_nestedOutput);
        }

        [Test]
        public void applies_if_view_output_is_null()
        {
            ClassUnderTest.Applies(_view).ShouldBeTrue();
            _view.Output = new StringWriter();
            ClassUnderTest.Applies(_view).ShouldBeFalse();
        }

        [Test]
        public void set_output_as_nested_output_writer()
        {
            TextWriter writer = new StringWriter();
            _nestedOutput.SetWriter(() => writer);
            ClassUnderTest.Modify(_view);
            _view.Output.ShouldEqual(writer);
        }
    }
}