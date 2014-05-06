using System;
using System.Linq;
using FubuMVC.Core.UI.Extensions;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Extensions
{
    [TestFixture]
    public class FilteredContentExtensionTester : InteractionContext<FilteredContentExtension<FilteredModel>>
    {
        private IFubuPage<FilteredModel> thePage;
        private Func<IFubuPage<FilteredModel>, bool> theFilter;
        private object[] theInnerExtensionObjects;

        protected override void beforeEach()
        {
            thePage = MockFor<IFubuPage<FilteredModel>>();
            theInnerExtensionObjects = new object[]{"<div/>", "<div />"};
            MockFor<IContentExtension<FilteredModel>>().Stub(x => x.GetExtensions(thePage)).Return(theInnerExtensionObjects);

            theFilter = MockRepository.GenerateMock<Func<IFubuPage<FilteredModel>, bool>>();
            Services.Inject(theFilter);
        }

        [Test]
        public void if_the_filter_returns_false_return_an_empty_enumerable()
        {
            theFilter.Stub(x => x.Invoke(thePage)).Return(false);

            ClassUnderTest.GetExtensions(thePage).Any().ShouldBeFalse();
        }

        [Test]
        public void if_the_filter_returns_try_return_the_inner_enumerable()
        {
            theFilter.Stub(x => x.Invoke(thePage)).Return(true);

            ClassUnderTest.GetExtensions(thePage).ShouldBeTheSameAs(theInnerExtensionObjects);
        }
    }

    public class FilteredModel
    {
        
    }
}