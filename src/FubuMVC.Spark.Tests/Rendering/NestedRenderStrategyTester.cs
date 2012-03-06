using System.IO;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class NestedRenderStrategyTester : InteractionContext<NestedRenderStrategy>
    {
        private NestedOutput _nestedOutput;
        private IRenderAction _renderAction;
        private TextWriter _writer;
        protected override void beforeEach()
        {
            _writer = new StringWriter();
            _nestedOutput = new NestedOutput();
            _renderAction = MockFor<IRenderAction>();
            _renderAction.Expect(x => x.RenderPartial());
            Services.Inject(_nestedOutput);
        }

        [Test]
        public void if_nested_output_is_active_applies_returns_true_otherwise_false()
        {
            ClassUnderTest.Applies().ShouldBeFalse();
            _nestedOutput.SetWriter(() => _writer);
            ClassUnderTest.Applies().ShouldBeTrue();
        }

        [Test]
        public void invokes_render_on_injected_render_action()
        {
            ClassUnderTest.Invoke(_renderAction);
            _renderAction.VerifyAllExpectations();
        }
    }
}