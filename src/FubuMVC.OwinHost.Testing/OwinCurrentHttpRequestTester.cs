using System.Collections.Generic;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwinCurrentHttpRequestTester
    {
        private Dictionary<string, string[]> headers;
        private OwinCurrentHttpRequest request;

        [SetUp]
        public void SetUp()
        {
            headers = new Dictionary<string, string[]>();

            var dict = new Dictionary<string, object>();
            dict.Add(OwinConstants.RequestHeadersKey, headers);

            request = new OwinCurrentHttpRequest(dict);
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