using System.Web;
using FubuCore.Binding;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class AntiForgeryServiceTester : InteractionContext<AntiForgeryService>
    {
        protected override void beforeEach()
        {
            MockFor<IFubuApplicationFiles>().Stub(r => r.GetApplicationPath()).Return("Path");
            MockFor<ICookies>().Stub(r => r.Get("Cookies")).Return(new Cookie());

            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName("Path")).Return("CookieName");

            MockFor<IAntiForgerySerializer>()
                .Stub(x => x.Serialize(default(AntiForgeryData))).IgnoreArguments().Return("Serialized!");
        }

        [Test]
        public void should_decode_the_cookie_value()
        {
            var cookie = new Cookie("CookieName", HttpUtility.UrlEncode("Some Value11"));
            MockFor<ICookies>().Stub(r => r.Get("CookieName")).Return(cookie);

            ClassUnderTest.GetCookieToken();

            MockFor<IAntiForgerySerializer>()
                .AssertWasCalled(x => x.Deserialize(HttpUtility.UrlDecode(cookie.Value)));
        }

        [Test]
        public void should_return_form_token_from_cookie_data()
        {
            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName()).Return("FormName");

            var input = new AntiForgeryData
            {
                Username = "CookieUser",
                Value = "12345"
            };
            FormToken formToken = ClassUnderTest.GetFormToken(input, "Salty");

            formToken.Name.ShouldEqual("FormName");
            formToken.TokenString.ShouldEqual("Serialized!");
        }

        [Test]
        public void should_set_cookie()
        {
            MockFor<IOutputWriter>().Expect(o => o.AppendCookie(default(Cookie))).IgnoreArguments();

            ClassUnderTest.SetCookieToken(null, null);

            MockFor<IOutputWriter>().VerifyAllExpectations();
        }
    }
}