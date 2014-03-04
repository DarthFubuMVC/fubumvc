using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class RenderActionTester : InteractionContext<RenderAction>
    {
        private IFubuSparkView _view;
        private IViewFactory _viewFactory;
        protected override void beforeEach()
        {
            _viewFactory = MockFor<IViewFactory>();
            _view = MockFor<IFubuSparkView>();
            _view.Expect(x => x.Render());
        }

        [Test]
        public void render_uses_view_from_factory_and_calls_to_render_method()
        {
            _viewFactory.Stub(x => x.GetView()).Return(_view);
            ClassUnderTest.Render();
            _view.VerifyAllExpectations();
        }


        [Test]
        public void render_partial_uses_partial_view_from_factory_and_calls_to_render_method()
        {
            _viewFactory.Stub(x => x.GetPartialView()).Return(_view);
            ClassUnderTest.RenderPartial();
            _view.VerifyAllExpectations();
        }
    }
}