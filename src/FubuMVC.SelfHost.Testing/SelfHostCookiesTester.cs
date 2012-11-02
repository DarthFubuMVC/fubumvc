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
        public void gets_the_cookie_with_multiple_cookie_states()
        {
            theMessage.Headers.Add("Cookie", "TestCookie=Hello;AnotherCookie=Goodbye");
            ClassUnderTest.Get("TestCookie").ShouldNotBeNull();
            ClassUnderTest.Get("AnotherCookie").ShouldNotBeNull();
        }

        [Test]
        public void gets_the_cookie_with_multiple_cookie_states_with_name_value_pairs()
        {
            theMessage.Headers.Add("Cookie", "TestCookie=a1=b1&a2=b2;AnotherCookie=y1=z1&y2=z2");
            ClassUnderTest.Get("TestCookie").ShouldNotBeNull().Values.AllKeys.ShouldHaveTheSameElementsAs("a1", "a2");
            ClassUnderTest.Get("AnotherCookie").ShouldNotBeNull().Values.AllKeys.ShouldHaveTheSameElementsAs("y1", "y2");
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
}