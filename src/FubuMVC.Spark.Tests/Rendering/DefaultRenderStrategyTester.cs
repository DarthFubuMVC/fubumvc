using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class DefaultRenderStrategyTester : InteractionContext<DefaultRenderStrategy>
    {
        private IRenderAction _renderAction;

        protected override void beforeEach()
        {
            _renderAction = MockFor<IRenderAction>();
            _renderAction.Expect(x => x.Render());
        }

        [Test]
        public void applies_returns_true()
        {
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