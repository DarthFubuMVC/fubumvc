using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{

    [TestFixture]
    public class when_activating_a_page_with_a_model
    {
        private IPageActivator activator;
        private FubuPage<AddressViewModel> page;

        [SetUp]
        public void SetUp()
        {
            activator = MockRepository.GenerateMock<IPageActivator>();
            page = new FubuPage<AddressViewModel>();

            RenderFubuViewBehavior.ActivateView(activator, page);
        }

        [Test]
        public void should_activate_the_page_by_the_closed_model_type()
        {
            Assert.Fail("NWO");
            //activator.AssertWasCalled(x => x.Activate<AddressViewModel>(page));
        }
    }


    [TestFixture]
    public class when_activating_a_page_without_a_model
    {
        private IPageActivator activator;
        private FubuPage page;

        [SetUp]
        public void SetUp()
        {
            activator = MockRepository.GenerateMock<IPageActivator>();
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
            Services.Inject<IViewEngine<IFubuPage>>(engine);

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
            engine.Page.AssertWasCalled(x => x.SetModel(MockFor<IFubuRequest>()));
        }
    }

    public class StubViewEngine : IViewEngine<IFubuPage>
    {
        private readonly IFubuPageWithModel _page = MockRepository.GenerateMock<IFubuPageWithModel>();
        private ViewPath _viewPath;

        public IFubuPageWithModel Page { get { return _page; } }
        public ViewPath ViewPath { get { return _viewPath; } }

        public void RenderView(ViewPath viewPath, Action<IFubuPage> configureView)
        {
            _viewPath = viewPath;
            configureView(_page);
        }
    }
}