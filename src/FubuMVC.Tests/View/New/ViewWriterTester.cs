using System;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.View.New;
using FubuMVC.Core.View.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using FubuMVC.Core;

namespace FubuMVC.Tests.View.New
{
    [TestFixture]
    public class ViewWriterTester : InteractionContext<ViewWriter<SomeTarget>>
    {
        private IRenderableView partialView;
        private IRenderableView fullView;

        protected override void beforeEach()
        {
            partialView = MockRepository.GenerateMock<IRenderableView>();
            fullView = MockRepository.GenerateMock<IRenderableView>();

            MockFor<IViewFactory>().Stub(x => x.GetPartialView()).Return(partialView);
            MockFor<IViewFactory>().Stub(x => x.GetView()).Return(fullView);
        }

        [Test]
        public void build_the_view_when_it_is_an_ajax_request()
        {
            MockFor<IRequestHeaders>().Stub(x => x.IsAjaxRequest()).Return(true);

            ClassUnderTest.BuildView().ShouldBeTheSameAs(partialView);
        }

        [Test]
        public void build_the_view_when_it_is_a_partial_request()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial())
                .Return(true);

            ClassUnderTest.BuildView().ShouldBeTheSameAs(partialView);
        }

        [Test]
        public void build_the_view_when_it_is_not_in_partial_or_ajax_request()
        {
            MockFor<IRequestHeaders>().Stub(x => x.IsAjaxRequest()).Return(false);
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial())
                .Return(false);

            ClassUnderTest.BuildView().ShouldBeTheSameAs(fullView);
        }
    }

    public class SomeTarget{}
}