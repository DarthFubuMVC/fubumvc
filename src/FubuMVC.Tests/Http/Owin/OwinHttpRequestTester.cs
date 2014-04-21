using System.Collections.Generic;
using FubuMVC.Core.Http.Owin;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinHttpRequestTester
    {
        private Dictionary<string, string[]> headers;
        private OwinHttpRequest request;

        [SetUp]
        public void SetUp()
        {
            headers = new Dictionary<string, string[]>();

            var dict = new Dictionary<string, object>();
            dict.Add(OwinConstants.RequestHeadersKey, headers);

            request = new OwinHttpRequest(dict);
        }

        [Test]
        public void server_root_round_trip()
        {
            request.FullUrl("http://server/foo/bar");
            request.FullUrl().ShouldEqual("http://server/foo/bar");

            request.RelativeUrl().ShouldEqual("foo/bar");
        }

        [Test]
        public void has_negative()
        {
            request.HasHeader("a").ShouldBeFalse();
        }

        [Test]
        public void has_positive()
        {
            headers.Add("a", new string[]{"1", "2"});

            request.HasHeader("a").ShouldBeTrue();
            request.HasHeader("A").ShouldBeTrue();
        }

        [Test]
        public void get()
        {
            headers.Add("a", new string[] { "1", "2" });
            request.GetHeader("a").ShouldHaveTheSameElementsAs("1", "2");
            request.GetHeader("A").ShouldHaveTheSameElementsAs("1", "2");
        }

        
    }
}