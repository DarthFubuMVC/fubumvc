using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class FormForExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            page = MockRepository.GenerateMock<IFubuPage>();
        }

        #endregion

        private IFubuPage page;

        public class AddressController
        {
            public Elements.AddressViewModel Address()
            {
                return new Elements.AddressViewModel();
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
            page.EndForm().ToString().ShouldEqual("</form>");
        }


        [Test]
        public void form_for_with_an_url()
        {
            page.Stub(x => x.Get<IHttpRequest>()).Return(OwinHttpRequest.ForTesting());

            page.FormFor("some action").Attr("action").ShouldEqual("/some action");
        }

    }

}