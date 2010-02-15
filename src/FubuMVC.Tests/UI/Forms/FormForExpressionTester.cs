using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.FakeControllers;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using FubuMVC.UI;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class FormForExpressionTester
    {
        private IUrlRegistry urls;
        private IFubuPage page;

        [SetUp]
        public void SetUp()
        {
            urls = MockRepository.GenerateMock<IUrlRegistry>();
            page = MockRepository.GenerateMock<IFubuPage>();

            page.Stub(x => x.Urls).Return(urls);
        }

        [Test]
        public void basic_form_for()
        {
            page.FormFor().ShouldNotBeNull().ShouldBeOfType<FormTag>();
        }

        [Test]
        public void form_for_with_an_input_model()
        {
            var model = new AddressViewModel();

            urls.Stub(x => x.UrlFor(model)).Return("the url for the action");

            var tag = page.FormFor(model);

            tag.Attr("action").ShouldEqual("the url for the action");
        }

    }

}