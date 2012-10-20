using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class SelfHostCookiesTester : InteractionContext<SelfHostCookies>
    {
        private HttpRequestMessage theMessage;
        private HttpResponseMessage theResponse;

        protected override void beforeEach()
        {
            theMessage = new HttpRequestMessage();
            theResponse = new HttpResponseMessage();

            Services.Inject(theMessage);
            Services.Inject(theResponse);
        }


        [Test]
        public void has_cookie()
        {
            theMessage.Headers.Add("Cookie", "TestCookie=Hello");
            ClassUnderTest.Has("TestCookie").ShouldBeTrue();
        }

        [Test]
        public void has_cookie_negative()
        {
            ClassUnderTest.Has("TestCookie").ShouldBeFalse();
        }

        [Test]
        public void gets_the_cookie()
        {
            theMessage.Headers.Add("Cookie", "TestCookie=Hello");
            ClassUnderTest.Get("TestCookie").ShouldNotBeNull();
        }

        [Test]
        public void get_just_returns_null_if_nothing_is_found()
        {
            ClassUnderTest.Get("TestCookie").ShouldBeNull();
        }

        [Test]
        public void gets_the_response_cookies()
        {
            theResponse.Headers.Add("Cookie", "TestCookie=Hello");
            ClassUnderTest.Response.Single().Name.ShouldEqual("TestCookie");
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