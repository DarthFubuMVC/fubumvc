using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class AjaxRenderStrategyTester : InteractionContext<AjaxRenderStrategy>
    {
        private InMemoryRequestData _requestData;
        private IRenderAction _renderAction;

        protected override void beforeEach()
        {
            _requestData = new InMemoryRequestData();
            _renderAction = MockFor<IRenderAction>();
            _renderAction.Expect(x => x.RenderPartial());
            Services.Inject<IRequestData>(_requestData);
        }

        [Test]
        public void if_is_ajax_request_applies_returns_true_otherwise_false()
        {
            ClassUnderTest.Applies().ShouldBeFalse();
            _requestData[AjaxExtensions.XRequestedWithHeader] = "XMLHttpRequest";
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