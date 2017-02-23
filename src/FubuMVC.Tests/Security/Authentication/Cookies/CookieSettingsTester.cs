using System;
using FubuMVC.Core.Security.Authentication.Cookies;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication.Cookies
{
    
    public class CookieSettingsTester
    {
        private CookieSettings theSettings = new CookieSettings();

        [Fact]
        public void defaults_to_the_default_cookie_name()
        {
            theSettings.Name.ShouldBe(CookieSettings.DefaultCookieName);
        }

        [Fact]
        public void creates_the_expiration_date()
        {
            theSettings.ExpirationInDays = 90;
            
            var theDate = DateTime.Today;

            theSettings.ExpirationFor(theDate).ShouldBe(theDate.AddDays(theSettings.ExpirationInDays));
        }

        [Fact]
        public void default_expiration_is_30_days()
        {
            theSettings.ExpirationInDays.ShouldBe(30);
        }

        [Fact]
        public void has_a_default_path()
        {
            theSettings.Path.ShouldBe("/");
        }
    }
}