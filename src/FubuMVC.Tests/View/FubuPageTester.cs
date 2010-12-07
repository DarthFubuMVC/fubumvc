using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.View;
using FubuMVC.StructureMap;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class FubuPageTester : InteractionContext<TestFubuPage>
    {
        private TestFubuPageViewModel _model;

        protected override void beforeEach()
        {
            _model = new TestFubuPageViewModel();
            var fubuRequest = MockFor<IFubuRequest>();
            fubuRequest.Stub(x => x.Get<TestFubuPageViewModel>()).Return(_model);

            ClassUnderTest.SetModel(fubuRequest);
        }

        [Test]
        public void get_service_caches()
        {
            var container =
                new Container(x => { x.For<IElementNamingConvention>().Use<DefaultElementNamingConvention>(); });

            var page = new FubuPage();

            page.ServiceLocator = new StructureMapServiceLocator(container);

            var convention = page.Get<IElementNamingConvention>().ShouldBeOfType<DefaultElementNamingConvention>();

            page.Get<IElementNamingConvention>().ShouldBeTheSameAs(convention);
        }

        [Test]
        public void set_model_should_set_the_model_type_to_the_request_model()
        {
            ClassUnderTest.Model.ShouldBeTheSameAs(_model);
        }
    }

    [TestFixture]
    public class FubuMasterPageTester : InteractionContext<TestFubuMasterPage>
    {
        private TestFubuMasterPage _masterPage;
        private List<TestFubuPageViewModel> _models;

        protected override void beforeEach()
        {
            _masterPage = new TestFubuMasterPage();
            _models = new List<TestFubuPageViewModel>
            {
                new TestFubuPageViewModel()
            };
            var page = new TestFubuPage();
            var serviceLocator = MockFor<IServiceLocator>();
            var fubuRequest = MockFor<IFubuRequest>();
            fubuRequest.Stub(x => x.Find<TestFubuPageViewModel>()).Return(_models);
            serviceLocator.Stub(x => x.GetInstance<IFubuRequest>()).Return(fubuRequest);
            page.ServiceLocator = serviceLocator;
            _masterPage.Page = page;
        }

        [Test]
        public void should_get_the_model_out_of_the_pages_service_locator()
        {
            _masterPage.Model.ShouldBeTheSameAs(_models.FirstOrDefault());
        }
    }

    public class TestFubuPageViewModel
    {
    }

    public class TestFubuPage : FubuPage<TestFubuPageViewModel>
    {
    }

    public class TestFubuMasterPage : FubuMasterPage<TestFubuPageViewModel>
    {
    }
}