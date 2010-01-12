using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Tests.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{

    [TestFixture]
    public class when_activating_a_page_with_a_model
    {
        private IViewActivator activator;
        private FubuPage<AddressViewModel> page;

        [SetUp]
        public void SetUp()
        {
            activator = MockRepository.GenerateMock<IViewActivator>();
            page = new FubuPage<AddressViewModel>();

            RenderFubuViewBehavior.ActivateView(activator, page);
        }

        [Test]
        public void should_activate_the_page_by_the_closed_model_type()
        {
            activator.AssertWasCalled(x => x.Activate<AddressViewModel>(page));
        }
    }


    [TestFixture]
    public class when_activating_a_page_without_a_model
    {
        private IViewActivator activator;
        private FubuPage page;

        [SetUp]
        public void SetUp()
        {
            activator = MockRepository.GenerateMock<IViewActivator>();
            page = new FubuPage();

            RenderFubuViewBehavior.ActivateView(activator, page);
        }

        [Test]
        public void should_activate_the_page_by_the_closed_model_type()
        {
            activator.AssertWasCalled(x => x.Activate(page));
        }
    }


    [TestFixture]
    public class when_rendering_a_fubu_view : InteractionContext<RenderFubuViewBehavior>
    {
        private ViewPath view;
        private StubViewEngine engine;

        protected override void beforeEach()
        {
            view = new ViewPath();
            Services.Inject(view);

            engine = new StubViewEngine();
            Services.Inject<IViewEngine<IFubuView>>(engine);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_call_to_the_view_engine_with_the_path()
        {
            engine.ViewPath.ShouldBeTheSameAs(view);
        }

        [Test]
        public void should_configure_the_fubu_view_returned()
        {
            engine.View.AssertWasCalled(x => x.SetModel(MockFor<IFubuRequest>()));
        }
    }

    public class StubViewEngine : IViewEngine<IFubuView>
    {
        private readonly IFubuViewWithModel _view = MockRepository.GenerateMock<IFubuViewWithModel>();
        private ViewPath _viewPath;

        public IFubuViewWithModel View { get { return _view; } }
        public ViewPath ViewPath { get { return _viewPath; } }

        public void RenderView(ViewPath viewPath, Action<IFubuView> configureView)
        {
            _viewPath = viewPath;
            configureView(_view);
        }
    }
}