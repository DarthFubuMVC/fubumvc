using System.IO;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class NestedOutputActivationTester : InteractionContext<NestedOutputActivation>
    {
        private NestedOutput _nestedOutput;
        private IFubuSparkView _view;
        protected override void beforeEach()
        {
            _nestedOutput = new NestedOutput();
            _view = MockFor<IFubuSparkView>();
            _view.Stub(x => x.Output).PropertyBehavior();
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
        public void sets_the_nested_output_writer_as_the_view_output()
        {
            _view.Output = new StringWriter();
            ClassUnderTest.Modify(_view);
            _nestedOutput.Writer.ShouldBeTheSameAs(_view.Output);
        }
    }
}