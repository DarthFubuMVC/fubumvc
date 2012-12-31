using System.Net.Http;
using System.Net.Http.Headers;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class SelfHostCurrentHttpRequestTester
    {
        private HttpRequestMessage theMessage;
        private SelfHostCurrentHttpRequest request;
        private HttpRequestHeaders headers;

        [SetUp]
        public void SetUp()
        {
            theMessage = new HttpRequestMessage{};
            headers = theMessage.Headers;
            request = new SelfHostCurrentHttpRequest(theMessage);
        }


        [Test]
        public void has_negative()
        {
            request.HasHeader("a").ShouldBeFalse();
        }

        [Test]
        public void has_positive()
        {
            headers.Add("a", new string[] { "1", "2" });

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