using System;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class CurrentHttpRequestExtensions_integration_with_OWIN_Tester
    {
        [Test]
        public void modified_since()
        {
            var time = new DateTime(2014, 1, 30, 12, 5, 6);

            new OwinHttpRequest()
                .IfModifiedSince(time)
                .IfModifiedSince()
                .ShouldEqual(time.ToUniversalTime());
        }

        [Test]
        public void un_modified_since()
        {
            var time = new DateTime(2014, 1, 30, 12, 5, 6);

            new OwinHttpRequest()
                .IfUnModifiedSince(time)
                .IfUnModifiedSince()
                .ShouldEqual(time.ToUniversalTime());
        }

        [Test]
        public void if_match()
        {
            new OwinHttpRequest().IfMatch("a,b, c")
                .IfMatch()
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void if_none_match()
        {
            new OwinHttpRequest().IfNoneMatch("a,b, c")
                .IfNoneMatch()
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void HttpMethodMatchesAny()
        {
            var request = new OwinHttpRequest().HttpMethod("POST");

            request.HttpMethodMatchesAny("get", "put").ShouldBeFalse();
            request.HttpMethodMatchesAny("get").ShouldBeFalse();
            request.HttpMethodMatchesAny("head", "PUT").ShouldBeFalse();

            request.HttpMethodMatchesAny("POST").ShouldBeTrue();
            request.HttpMethodMatchesAny("POST", "get").ShouldBeTrue();
            request.HttpMethodMatchesAny("post", "get").ShouldBeTrue();
            request.HttpMethodMatchesAny("Post", "get").ShouldBeTrue();
        }
    }
}