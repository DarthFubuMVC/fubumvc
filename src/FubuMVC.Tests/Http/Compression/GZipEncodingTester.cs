using System.IO;
using System.IO.Compression;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Compression
{
    
    public class GZipEncodingTester
    {
        private GZipHttpContentEncoding theEncoding = new GZipHttpContentEncoding();

        [Fact]
        public void matches_gzip()
        {
            theEncoding.MatchingEncoding.ShouldBe(ContentEncoding.GZip);
        }

        [Fact]
        public void compresses_the_stream()
        {
            var original = "Testing...".AsStream().As<MemoryStream>();
            var compressed = theEncoding.Encode(original);

            // Hit this hard w/ the integration tests
            compressed.ShouldBeOfType<GZipStream>();
        }
    }
}