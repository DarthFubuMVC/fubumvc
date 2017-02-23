using FubuMVC.Core.Http.Compression;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Compression
{
    
    public class ContentEncodingTester
    {
        private ContentEncoding theEncoding = ContentEncoding.GZip;

        [Fact]
        public void equals()
        {
            theEncoding.ShouldBe(ContentEncoding.GZip);
        }

        [Fact]
        public void simple_match()
        {
            theEncoding.Matches("gzip").ShouldBeTrue();
        }

        [Fact]
        public void matches_case_insensitive()
        {
            theEncoding.Matches("GZip").ShouldBeTrue();
        }

        [Fact]
        public void matches_negative()
        {
            theEncoding.Matches("deflate").ShouldBeFalse();
        }
    }
}