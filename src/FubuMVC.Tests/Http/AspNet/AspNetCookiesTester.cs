using System.IO;
using System.Web;
using FubuMVC.Core.Http.AspNet;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.AspNet
{
    [TestFixture]
    public class AspNetCookiesTester : InteractionContext<AspNetCookies>
    {
        private HttpCookieCollection theRequest;
        private HttpCookieCollection theResponse;

        private HttpCookie c1;
        private HttpCookie c2;

        protected override void beforeEach()
        {
            theRequest = new HttpCookieCollection();
            theResponse = new HttpCookieCollection();

            c1 = new HttpCookie("Test1");
            c2 = new HttpCookie("Test2");

            theRequest.Add(c1);
            theResponse.Add(c2);

            MockFor<HttpContextBase>().Stub(x => x.Request).Return(MockFor<HttpRequestBase>());
            MockFor<HttpContextBase>().Stub(x => x.Response).Return(MockFor<HttpResponseBase>());

            MockFor<HttpRequestBase>().Stub(x => x.Cookies).Return(theRequest);
            MockFor<HttpResponseBase>().Stub(x => x.Cookies).Return(theResponse);
        }

        [Test]
        public void gets_the_request_cookies()
        {
            ClassUnderTest.Request.ShouldHaveTheSameElementsAs(c1);
        }

        [Test]
        public void gets_the_response_cookies()
        {
            ClassUnderTest.Response.ShouldHaveTheSameElementsAs(c2);
        }
    }
}