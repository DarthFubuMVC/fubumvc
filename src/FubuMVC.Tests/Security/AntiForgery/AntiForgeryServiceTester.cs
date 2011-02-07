using System.Security.Principal;
using System.Web;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.AntiForgery;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.AntiForgery
{
	[TestFixture]
	public class AntiForgeryServiceTester:InteractionContext<AntiForgeryService>
	{
		private HttpCookieCollection _requestCookies;
		private HttpCookieCollection _responseCookies;

		protected override void beforeEach()
		{

			//TODO: Stop using HttpContext
			_requestCookies = new HttpCookieCollection();
			_responseCookies = new HttpCookieCollection();
			MockFor<HttpContextBase>().Stub(x => x.Request).Return(MockFor<HttpRequestBase>());
			MockFor<HttpContextBase>().Stub(x => x.Response).Return(MockFor<HttpResponseBase>());
			MockFor<HttpRequestBase>().Stub(x => x.ApplicationPath).Return("Path");
			MockFor<HttpRequestBase>().Stub(x => x.Cookies).Return(_requestCookies);
			MockFor<HttpResponseBase>().Stub(x => x.Cookies).Return(_responseCookies);

			MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName("Path")).Return("CookieName");

			MockFor<IAntiForgerySerializer>()
				.Stub(x => x.Serialize(default(AntiForgeryData))).IgnoreArguments().Return("Serialized!");

		}

		[Test]
		public void should_set_cookie()
		{
			ClassUnderTest.SetCookieToken(null, null);

			_responseCookies.Count.ShouldEqual(1);
		}


		[Test]
		public void should_return_form_token_from()
		{

			MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName()).Return("FormName");

			var input = new AntiForgeryData
			{
				Username = "CookieUser",
				Value = "12345"
			};
			var formToken = ClassUnderTest.GetFormToken(input, "Salty");

			formToken.Name.ShouldEqual("FormName");
			formToken.TokenString.ShouldEqual("Serialized!");

		}
		
	}
}