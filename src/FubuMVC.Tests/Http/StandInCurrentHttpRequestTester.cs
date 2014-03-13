using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class StandInCurrentHttpRequestTester
    {
        private StandInHttpRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theRequest = new StandInHttpRequest();
        }

        [Test]
        public void fullurl_returns_stubfullurl_field()
        {
            theRequest.FullUrl().ShouldEqual(theRequest.StubFullUrl);
        }

        [Test]
        public void rawurl_returns_therawurl_field()
        {
            theRequest.RawUrl().ShouldEqual(theRequest.TheRawUrl);
        }

        [Test]
        public void relativeurl_returns_therelativeurl_field()
        {
            theRequest.RelativeUrl().ShouldEqual(theRequest.TheRelativeUrl);
        }

        [Test]
        public void httpmethod_returns_thehttpmethod_field()
        {
            theRequest.HttpMethod().ShouldEqual(theRequest.TheHttpMethod);
        }
    }
}