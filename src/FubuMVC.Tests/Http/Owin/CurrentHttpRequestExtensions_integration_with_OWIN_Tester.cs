using System;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin
{
    
    public class CurrentHttpRequestExtensions_integration_with_OWIN_Tester
    {
        [Fact]
        public void modified_since()
        {
            var time = new DateTime(2014, 1, 30, 12, 5, 6);

            new OwinHttpRequest()
                .IfModifiedSince(time)
                .IfModifiedSince()
                .ShouldBe(time.ToUniversalTime());
        }

        [Fact]
        public void un_modified_since()
        {
            var time = new DateTime(2014, 1, 30, 12, 5, 6);

            new OwinHttpRequest()
                .IfUnModifiedSince(time)
                .IfUnModifiedSince()
                .ShouldBe(time.ToUniversalTime());
        }

        [Fact]
        public void if_match()
        {
            new OwinHttpRequest().IfMatch("a,b, c")
                .IfMatch()
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Fact]
        public void if_none_match()
        {
            new OwinHttpRequest().IfNoneMatch("a,b, c")
                .IfNoneMatch()
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Fact]
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