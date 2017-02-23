using System.Collections.Generic;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin
{
    
    public class OwinHttpRequestTester
    {
        private Dictionary<string, string[]> headers;
        private OwinHttpRequest request;

        public OwinHttpRequestTester()
        {
            headers = new Dictionary<string, string[]>();

            var dict = new Dictionary<string, object>();
            dict.Add(OwinConstants.RequestHeadersKey, headers);

            request = new OwinHttpRequest(dict);
        }

        [Fact]
        public void server_root_round_trip()
        {
            request.FullUrl("http://server/foo/bar");
            request.FullUrl().ShouldBe("http://server/foo/bar");

            request.RelativeUrl().ShouldBe("foo/bar");
        }


        [Fact]
        public void server_root_round_trip_with_querystring()
        {
            request.FullUrl("http://server/foo/bar?foo=bar");
            request.FullUrl().ShouldBe("http://server/foo/bar?foo=bar");

            request.RelativeUrl().ShouldBe("foo/bar?foo=bar");
        }

        [Fact]
        public void has_negative()
        {
            request.HasHeader("a").ShouldBeFalse();
        }

        [Fact]
        public void has_positive()
        {
            headers.Add("a", new string[]{"1", "2"});

            request.HasHeader("a").ShouldBeTrue();
            request.HasHeader("A").ShouldBeTrue();
        }

        [Fact]
        public void get()
        {
            headers.Add("a", new string[] { "1", "2" });
            request.GetHeader("a").ShouldHaveTheSameElementsAs("1", "2");
            request.GetHeader("A").ShouldHaveTheSameElementsAs("1", "2");
        }

        
    }
}