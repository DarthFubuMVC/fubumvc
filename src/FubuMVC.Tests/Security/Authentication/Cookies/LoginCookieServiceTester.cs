using System;
using System.Linq;
using FubuCore.Dates;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication.Cookies
{
    [TestFixture]
    public class LoginCookieServiceTester : InteractionContext<LoginCookieService>
    {
        private CookieSettings theSettings;
        private ISystemTime theSystemTime;
        private DateTime today;

        protected override void beforeEach()
        {
            theSettings = new CookieSettings();
            theSystemTime = MockRepository.GenerateStub<ISystemTime>();

            today = DateTime.Today;
            theSystemTime.Stub(x => x.UtcNow()).Return(today);

            Services.Inject(theSettings);
        }

        private Cookie theCookie { get { return ClassUnderTest.CreateCookie(theSystemTime); } }

        [Test]
        public void sets_the_name_from_the_settings()
        {
            theCookie.States.First().Name.ShouldBe(theSettings.Name);
        }

        [Test]
        public void sets_the_domain_if_specified_in_the_settings()
        {
            theSettings.Domain = "test.com";
            theCookie.Domain.ShouldBe(theSettings.Domain);
        }

        [Test]
        public void sets_the_path_if_specified_in_the_settings()
        {
            theSettings.Path = "/test";
            theCookie.Path.ShouldBe(theSettings.Path);
        }

        [Test]
        public void does_not_set_the_path_if_not_specified_in_the_settings()
        {
            theSettings.Path = null;
            theCookie.Path.ShouldBeNull();
        }

        [Test]
        public void sets_the_secure_flag_from_the_settings()
        {
            theSettings.Secure = false;
            theCookie.Secure.ShouldBeFalse();

            theSettings.Secure = true;
            theCookie.Secure.ShouldBeTrue();
        }

        [Test]
        public void sets_the_httponly_flag_from_the_settings()
        {
            theSettings.HttpOnly = false;
            theCookie.HttpOnly.ShouldBeFalse();

            theSettings.HttpOnly = true;
            theCookie.HttpOnly.ShouldBeTrue();
        }

        [Test]
        public void sets_the_expiration_date()
        {
            var actual = theCookie.Expires.Value.UtcDateTime;
            actual.ShouldBe(theSettings.ExpirationFor(today).ToUniversalTime());
        }
    }

    [TestFixture]
    public class when_getting_the_current_cookie : InteractionContext<LoginCookieService>
    {
        private Cookie theCookie;

        protected override void beforeEach()
        {
            theCookie = new Cookie(CookieSettings.DefaultCookieName);

            MockFor<ICookies>().Stub(x => x.Get(CookieSettings.DefaultCookieName)).Return(theCookie);
        }

        [Test]
        public void finds_the_cookie_by_name_from_the_settings()
        {
            ClassUnderTest.Current().ShouldBeTheSameAs(theCookie);
        }
    }

    [TestFixture]
    public class when_updating_the_login_cookie : InteractionContext<LoginCookieService>
    {
        private Cookie theCookie;

        protected override void beforeEach()
        {
            theCookie = new Cookie("Test");
            ClassUnderTest.Update(theCookie);
        }

        [Test]
        public void adds_the_cookie_to_the_response()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendCookie(theCookie));
        }
    }
}