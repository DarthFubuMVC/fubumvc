using System;
using System.Linq.Expressions;
using FubuMVC.Core.Http;
using FubuMVC.Core.UI.Testing.Elements;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Core.UI.Testing.Forms
{
    [TestFixture]
    public class FormForExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            urls = MockRepository.GenerateMock<IUrlRegistry>();
            page = MockRepository.GenerateMock<IFubuPage>();

            page.Stub(x => x.Urls).Return(urls);
        }

        #endregion

        private IUrlRegistry urls;
        private IFubuPage page;

        public class AddressController
        {
            public AddressViewModel Address()
            {
                return new AddressViewModel();
            }
        }

        [Test]
        public void basic_form_for()
        {
            page.FormFor().ShouldNotBeNull().ShouldBeOfType<FormTag>();
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
            urls.Stub(x => x.UrlFor(expression, "POST")).Return("the url for the action");
            page.FormFor(expression).Attr("action").ShouldEqual("the url for the action");
        }

        [Test]
        public void form_for_with_a_new_input_model()
        {
            urls.Stub(x => x.UrlFor(Arg<AddressViewModel>.Is.Anything, Arg<string>.Is.Equal("POST"))).Return(
                "the url for the action");

            FormTag tag = page.FormFor<AddressViewModel>();

            tag.Attr("action").ShouldEqual("the url for the action");
        }

        [Test]
        public void form_for_with_an_input_model()
        {
            var model = new AddressViewModel();

            urls.Stub(x => x.UrlFor(model, "POST")).Return("the url for the action");

            FormTag tag = page.FormFor(model);

            tag.Attr("action").ShouldEqual("the url for the action");
        }

        [Test]
        public void form_for_with_an_url()
        {
            page.Stub(x => x.Get<ICurrentHttpRequest>()).Return(new StubCurrentHttpRequest
            {
                TheApplicationRoot = "http://server"
            });

            page.FormFor("some action").Attr("action").ShouldEqual("http://server/some action");
        }

        [Test]
        public void form_for_with_an_url_object()
        {
            object url = "some url";
            page.FormFor(url).Attr("action").ShouldEqual("some url");
        }
    }

    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string FullUrl()
        {
            return StubFullUrl;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(TheApplicationRoot);
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}