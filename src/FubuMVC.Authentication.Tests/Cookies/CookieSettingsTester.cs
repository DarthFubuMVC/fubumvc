using System;
using System.Diagnostics;
using System.Web;
using FubuMVC.Authentication.Cookies;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Authentication.Tests.Cookies
{
    [TestFixture]
    public class CookieSettingsTester
    {
        private CookieSettings theSettings;

        [SetUp]
        public void SetUp()
        {
            theSettings = new CookieSettings();
        }

        [Test]
        public void defaults_to_the_default_cookie_name()
        {
            theSettings.Name.ShouldEqual(CookieSettings.DefaultCookieName);
        }

        [Test]
        public void creates_the_expiration_date()
        {
            theSettings.ExpirationInDays = 90;
            
            var theDate = DateTime.Today;

            theSettings.ExpirationFor(theDate).ShouldEqual(theDate.AddDays(theSettings.ExpirationInDays));
        }

        [Test]
        public void default_expiration_is_30_days()
        {
            theSettings.ExpirationInDays.ShouldEqual(30);
        }

        [Test]
        public void has_a_default_path()
        {
            theSettings.Path.ShouldEqual("/");
        }
    }
}