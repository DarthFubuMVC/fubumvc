using System.IO;
using System.IO.Compression;
using FubuCore;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
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
            theEncoding.MatchingEncoding.ShouldEqual(ContentEncoding.GZip);
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