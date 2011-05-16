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

        protected override void beforeEach()
        {
            _view = MockFor<IFubuSparkView>();

            var viewFactory = MockFor<IViewFactory>();
            viewFactory.Stub(x => x.GetView()).Return(_view);

            _view.Expect(x => x.Render());

            ClassUnderTest.Render();
        }

        [Test]
        public void renders_view_from_factory()
        {
            _view.VerifyAllExpectations();
        }
    }
}