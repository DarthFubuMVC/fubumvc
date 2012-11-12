using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using FubuCore.Binding;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class AntiForgeryValidatorTester : InteractionContext<AntiForgeryValidator>
    {
        private NameValueCollection _form;
        private HttpCookieCollection _cookies;
        private AntiForgeryData _cookieToken;
        private AntiForgeryData _formToken;

        protected override void beforeEach()
        {
            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName()).Return("FormName");
            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName("String")).IgnoreArguments().Return(
                "CookieName");

            MockFor<IRequestData>().Stub(x => x.Value("ApplicationPath")).Return("Path");

            _form = new NameValueCollection {{"FormName", "FormValue"}};
            MockFor<IRequestData>().Stub(x => x.Value("Form")).Return(_form);

            _cookies = new HttpCookieCollection {new HttpCookie("CookieName", "CookieValue")};
            MockFor<IRequestData>().Stub(x => x.Value("Cookies")).Return(_cookies);

            _formToken = new AntiForgeryData
            {
                CreationDate = new DateTime(2010, 12, 12),
                Salt = "Salty",
                Username = "User",
                Value = "12345"
            };

            _cookieToken = new AntiForgeryData
            {
                CreationDate = new DateTime(2010, 12, 12),
                Salt = "Salty",
                Username = "User",
                Value = "12345"
            };
            MockFor<IAntiForgerySerializer>().Stub(x => x.Deserialize("CookieValue")).Return(_cookieToken);
            MockFor<IAntiForgerySerializer>().Stub(x => x.Deserialize("FormValue")).Return(_formToken);


            MockFor<IPrincipal>().Stub(x => x.Identity).Return(MockFor<IIdentity>());
            MockFor<ISecurityContext>().Stub(x => x.CurrentUser).Return(MockFor<IPrincipal>());
        }

        private void SetupIdentity(bool authenticated, string name)
        {
            MockFor<IIdentity>().Stub(x => x.IsAuthenticated).Return(authenticated);
            MockFor<IIdentity>().Stub(x => x.Name).Return(name);
        }


        [Test]
        public void should_not_validate_with_incorrect_user()
        {
            SetupIdentity(true, "DifferentUser");
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_not_validate_with_nonmatching_token_values()
        {
            SetupIdentity(true, "User");

            _formToken.Value = "I don't match!";

            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }


        [Test]
        public void should_not_validate_with_unauthenticated_user()
        {
            SetupIdentity(false, "User");
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_not_validate_without_cookie_token()
        {
            SetupIdentity(true, "User");
            _cookies.Clear();
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_not_validate_without_form_token()
        {
            SetupIdentity(true, "User");
            _form.Clear();
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_validate_with_correct_request_data()
        {
            MockFor<IIdentity>().Stub(x => x.IsAuthenticated).Return(true);
            MockFor<IIdentity>().Stub(x => x.Name).Return("User");
            ClassUnderTest.Validate("Salty").ShouldBeTrue();
        }
    }
}