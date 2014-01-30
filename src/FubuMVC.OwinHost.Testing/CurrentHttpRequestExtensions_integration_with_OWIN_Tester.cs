using System;
using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class CurrentHttpRequestExtensions_integration_with_OWIN_Tester
    {
        [Test]
        public void modified_since()
        {
            var time = new DateTime(2014, 1, 30, 12, 5, 6);

            new OwinCurrentHttpRequest()
                .IfModifiedSince(time)
                .IfModifiedSince()
                .ShouldEqual(time.ToUniversalTime());
        }

        [Test]
        public void un_modified_since()
        {
            var time = new DateTime(2014, 1, 30, 12, 5, 6);

            new OwinCurrentHttpRequest()
                .IfUnModifiedSince(time)
                .IfUnModifiedSince()
                .ShouldEqual(time.ToUniversalTime());
        }

        [Test]
        public void if_match()
        {
            new OwinCurrentHttpRequest().IfMatch("a,b, c")
                .IfMatch()
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void if_none_match()
        {
            new OwinCurrentHttpRequest().IfNoneMatch("a,b, c")
                .IfNoneMatch()
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void HttpMethodMatchesAny()
        {
            var request = new OwinCurrentHttpRequest().HttpMethod("POST");

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