using System;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Web;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class SelfHostCookiesTester
    {
        private CookieHeaderValue theOriginal;
        private NameValueCollection theValues;
        private DateTime theExpiration;

        [SetUp]
        public void SetUp()
        {
            theValues = new NameValueCollection();
            theValues.Add("x", "aaa");
            theValues.Add("y", "bbb");

            theExpiration = DateTime.Today.ToUniversalTime().AddDays(1);

            theOriginal = new CookieHeaderValue("Test", theValues);
            theOriginal.Domain = "fubu-project.org";
            theOriginal.Path = "/";
            theOriginal.HttpOnly = true;
            theOriginal.Secure = true;
            theOriginal.Expires = new DateTimeOffset(theExpiration);
        }

        [Test]
        public void maps_the_cookie_values()
        {
            var cookie = SelfHostCookies.CookieFor(theOriginal);

            cookie.Domain.ShouldEqual(theOriginal.Domain);
            cookie.Path.ShouldEqual(theOriginal.Path);
            cookie.Expires.ShouldEqual(theExpiration);

            cookie.Name.ShouldEqual("Test");

            cookie.HttpOnly.ShouldEqual(theOriginal.HttpOnly);
            cookie.Secure.ShouldEqual(theOriginal.Secure);
        }

        
    }

    [TestFixture]
    public class determining_the_name_of_a_CookieHeaderValue
    {
        [Test]
        public void name_for_a_single_cookie_state()
        {
            var value = new CookieHeaderValue("Test", "Value");
            SelfHostCookies.DetermineName(value).ShouldEqual("Test");
        }

        [Test]
        public void throws_when_no_cookie_state_is_found()
        {
            var value = new CookieHeaderValue("Test", "Value");
            value.Cookies.Clear();

            Exception<ArgumentException>
                .ShouldBeThrownBy(() =>SelfHostCookies.DetermineName(value));
        }

        [Test]
        public void name_for_multiple_cookie_states()
        {
            var theValues = new NameValueCollection();
            theValues.Add("x", "aaa");
            theValues.Add("y", "bbb");

            var value = new CookieHeaderValue("Test", theValues);
            
            SelfHostCookies.DetermineName(value).ShouldEqual("Test");
        }
    }

    [TestFixture]
    public class determining_the_value_of_a_CookieHeaderValue
    {
        [Test]
        public void value_for_a_single_cookie_state()
        {
            var value = new CookieHeaderValue("Test", "Value");
            var cookie = new HttpCookie("Test");

            SelfHostCookies.FillValues(value, cookie);

            cookie.Value.ShouldEqual("Value");
        }

        [Test]
        public void throws_when_no_cookie_state_is_found()
        {
            var value = new CookieHeaderValue("Test", "Value");
            value.Cookies.Clear();

            var cookie = new HttpCookie("Test");

            Exception<ArgumentException>
                .ShouldBeThrownBy(() => SelfHostCookies.FillValues(value, cookie));
        }

        [Test]
        public void values_for_multiple_cookie_states()
        {
            var theValues = new NameValueCollection();
            theValues.Add("x", "aaa");
            theValues.Add("y", "bbb");

            var value = new CookieHeaderValue("Test", theValues);
            var cookie = new HttpCookie("Test");

            SelfHostCookies.FillValues(value, cookie);

            cookie.Values["x"].ShouldEqual("aaa");
            cookie.Values["y"].ShouldEqual("bbb");
        }
    }
}