using System.IO;
using System.IO.Compression;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Compression
{
    
    public class DeflateEncodingTester
    {
        private DeflateHttpContentEncoding theEncoding = new DeflateHttpContentEncoding();

        [Fact]
        public void matches_gzip()
        {
            theEncoding.MatchingEncoding.ShouldBe(ContentEncoding.Deflate);
        }

        [Fact]
        public void compresses_the_stream_using_deflate()
        {
            var original = "Testing...".AsStream().As<MemoryStream>();
            var compressed = theEncoding.Encode(original);

            compressed.ShouldBeOfType<DeflateStream>();
        }
    }
}