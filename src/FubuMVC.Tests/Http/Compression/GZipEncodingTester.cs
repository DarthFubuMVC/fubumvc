using System.IO;
using System.IO.Compression;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Compression
{
    [TestFixture]
    public class GZipEncodingTester
    {
        private GZipHttpContentEncoding theEncoding;

        [SetUp]
        public void SetUp()
        {
            theEncoding = new GZipHttpContentEncoding();
        }

        [Test]
        public void matches_gzip()
        {
            theEncoding.MatchingEncoding.ShouldBe(ContentEncoding.GZip);
        }

        [Test]
        public void compresses_the_stream()
        {
            var original = "Testing...".AsStream().As<MemoryStream>();
            var compressed = theEncoding.Encode(original);

            // Hit this hard w/ the integration tests
            compressed.ShouldBeOfType<GZipStream>();
        }
    }
}