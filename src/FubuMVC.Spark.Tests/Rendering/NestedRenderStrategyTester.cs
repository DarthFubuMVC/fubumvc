using System.IO;
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

        protected override void beforeEach()
        {
            _nestedOutput = new NestedOutput();
            _renderAction = MockFor<IRenderAction>();
            _renderAction.Expect(x => x.Render());
            Services.Inject(_nestedOutput);
        }

        [Test]
        public void if_nested_output_is_active_applies_returns_true_otherwise_false()
        {
            ClassUnderTest.Applies().ShouldBeFalse();
            _nestedOutput.SetView(() => MockFor<IFubuSparkView>());
            ClassUnderTest.Applies().ShouldBeTrue();
        }

        [Test]
        public void invokes_render_on_injected_render_action()
        {
            ClassUnderTest.Invoke();
            _renderAction.VerifyAllExpectations();
        }
    }
}