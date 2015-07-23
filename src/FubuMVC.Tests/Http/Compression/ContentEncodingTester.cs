using FubuMVC.Core.Http.Compression;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Compression
{
    [TestFixture]
    public class ContentEncodingTester
    {
        private ContentEncoding theEncoding;

        [SetUp]
        public void SetUp()
        {
            theEncoding = ContentEncoding.GZip;
        }

        [Test]
        public void equals()
        {
            theEncoding.ShouldBe(ContentEncoding.GZip);
        }

        [Test]
        public void simple_match()
        {
            theEncoding.Matches("gzip").ShouldBeTrue();
        }

        [Test]
        public void matches_case_insensitive()
        {
            theEncoding.Matches("GZip").ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            theEncoding.Matches("deflate").ShouldBeFalse();
        }
    }
}