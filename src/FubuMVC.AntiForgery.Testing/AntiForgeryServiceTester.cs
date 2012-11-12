using System.Web;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;
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
            MockFor<IRequestData>().Stub(r => r.Value("ApplicationPath")).Return("Path");
            MockFor<IRequestData>().Stub(r => r.Value("Cookies")).Return(new HttpCookieCollection());

            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName("Path")).Return("CookieName");

            MockFor<IAntiForgerySerializer>()
                .Stub(x => x.Serialize(default(AntiForgeryData))).IgnoreArguments().Return("Serialized!");
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
            MockFor<IOutputWriter>().Expect(o => o.AppendCookie(default(HttpCookie))).IgnoreArguments();

            ClassUnderTest.SetCookieToken(null, null);

            MockFor<IOutputWriter>().VerifyAllExpectations();
        }
    }
}