using System;
using System.Globalization;
using FubuMVC.Core.Http.Cookies;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http.Cookies
{
    [TestFixture]
    public class CookieTester
    {
        

        [Test]
        public void matches()
        {
            var cookie = new Cookie();
            cookie.Matches("a").ShouldBeFalse();

            cookie.Add(new CookieState("something"));

            cookie.Matches("a").ShouldBeFalse();
            cookie.Matches("something").ShouldBeTrue();

            cookie.Add(new CookieState("Else"));
            cookie.Matches("something").ShouldBeTrue();
            cookie.Matches("else").ShouldBeTrue();
            cookie.Matches("a").ShouldBeFalse();
        }

        [Test]
        public void to_string_smoke_testing()
        {
            roundTrip("a=1; b=2");
            roundTrip("a=1; b=b1=1&b2=2");
            roundTrip("a=1; b=b1=1&b2=2; httponly");
            roundTrip("a=1; b=b1=1&b2=2; secure; httponly");
            roundTrip("a=1; b=b1=1&b2=2; secure");
            roundTrip("a=1; domain=http://cnn.com");
            roundTrip("a=1; max-age=5; domain=http://cnn.com");
            roundTrip("a=1; domain=http://cnn.com; path=foo");
            roundTrip("a=1; max-age=5; domain=http://cnn.com; path=foo; secure; httponly");
        }

        [Test]
        public void to_string_expires()
        {
            var cookie = new Cookie("something");
            cookie.Expires = DateTime.Today.AddHours(10);

            cookie.ToString().ShouldContain("expires=" + cookie.Expires.Value.ToString("r", CultureInfo.InvariantCulture));
        }

        private void roundTrip(string text)
        {
            var cookie = CookieParser.ToCookie(text);
            cookie.ToString().ShouldEqual(text);
        }
    }
}