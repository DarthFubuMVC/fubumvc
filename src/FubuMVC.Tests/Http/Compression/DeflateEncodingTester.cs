using System.IO;
using System.IO.Compression;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Compression
{
    [TestFixture]
    public class DeflateEncodingTester
    {
        private DeflateHttpContentEncoding theEncoding;

        [SetUp]
        public void SetUp()
        {
            theEncoding = new DeflateHttpContentEncoding();
        }

        [Test]
        public void matches_gzip()
        {
            theEncoding.MatchingEncoding.ShouldBe(ContentEncoding.Deflate);
        }

        [Test]
        public void compresses_the_stream_using_deflate()
        {
            var original = "Testing...".AsStream().As<MemoryStream>();
            var compressed = theEncoding.Encode(original);

            compressed.ShouldBeOfType<DeflateStream>();
        }
    }
}