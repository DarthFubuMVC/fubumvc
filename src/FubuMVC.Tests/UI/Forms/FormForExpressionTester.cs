using System;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

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

        [Test]
        public void form_for_with_a_new_input_model()
        {
            urls.Stub(x => x.UrlFor(Arg<AddressViewModel>.Is.Anything)).Return("the url for the action");

            var tag = page.FormFor<AddressViewModel>();

            tag.Attr("action").ShouldEqual("the url for the action");
        }

        [Test]
        public void form_for_with_an_url()
        {
            UrlContext.Stub("some url");
            page.FormFor("some action").Attr("action").ShouldEqual("some url/some action");
        }

        [Test]
        public void form_for_with_an_url_object()
        {
            object url = "some url";
            page.FormFor(url).Attr("action").ShouldEqual("some url");
        }

        [Test]
        public void end_form()
        {
            page.EndForm().ShouldEqual("</form>");
        }

        [Test]
        public void form_for_from_controller_expression()
        {
            Expression<Action<AddressController>> expression = c => c.Address();
            urls.Stub(x => x.UrlFor(expression)).Return("the url for the action");
            page.FormFor(expression).Attr("action").ShouldEqual("the url for the action");
        }

        public class AddressController
        {
            public AddressViewModel Address()
            {
                return new AddressViewModel();
            }
        }
    }

}