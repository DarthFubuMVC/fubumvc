using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class HttpContentEncodersTester
    {
        private IHttpContentEncoding e1;
        private IHttpContentEncoding e2;
        private HttpContentEncoders theEncoders;

        [SetUp]
        public void SetUp()
        {
            e1 = MockRepository.GenerateStub<IHttpContentEncoding>();
            e2 = MockRepository.GenerateStub<IHttpContentEncoding>();

            e1.Stub(x => x.MatchingEncoding).Return(ContentEncoding.GZip);
            e2.Stub(x => x.MatchingEncoding).Return(ContentEncoding.Deflate);

            theEncoders = new HttpContentEncoders(new[] { e1, e2 });
        }

        [Test]
        public void simple_match_on_first()
        {
            theEncoders.MatchFor("gzip, deflate").ShouldEqual(e1);
        }

        [Test]
        public void match_on_last()
        {
            theEncoders.MatchFor("unknown, deflate").ShouldEqual(e2);
        }

        [Test]
        public void no_match_returns_passthrough()
        {
            theEncoders.MatchFor("unknown1, unknown2").ShouldBeOfType<HttpContentEncoders.PassthroughEncoding>();
        }
    }
}