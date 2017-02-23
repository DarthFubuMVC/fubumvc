using FubuMVC.Core.Http.Compression;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Compression
{
    
    public class HttpContentEncodersTester
    {
        private IHttpContentEncoding e1;
        private IHttpContentEncoding e2;
        private HttpContentEncoders theEncoders;

        public HttpContentEncodersTester()
        {
            e1 = MockRepository.GenerateStub<IHttpContentEncoding>();
            e2 = MockRepository.GenerateStub<IHttpContentEncoding>();

            e1.Stub(x => x.MatchingEncoding).Return(ContentEncoding.GZip);
            e2.Stub(x => x.MatchingEncoding).Return(ContentEncoding.Deflate);

            theEncoders = new HttpContentEncoders(new[] { e1, e2 });
        }


        [Fact]
        public void simple_match_on_first()
        {
            theEncoders.MatchFor("gzip, deflate").ShouldBe(e1);
        }

        [Fact]
        public void match_on_last()
        {
            theEncoders.MatchFor("unknown, deflate").ShouldBe(e2);
        }

        [Fact]
        public void no_match_returns_passthrough()
        {
            theEncoders.MatchFor("unknown1, unknown2").ShouldBeOfType<HttpContentEncoders.PassthroughEncoding>();
        }
    }
}