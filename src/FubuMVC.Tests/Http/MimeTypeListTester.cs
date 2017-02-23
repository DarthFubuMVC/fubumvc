using FubuMVC.Core.Http;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http
{
    
    public class MimeTypeListTester
    {
        [Fact]
        public void build_from_string()
        {
            var list = new MimeTypeList("text/json");
            list.ShouldHaveTheSameElementsAs("text/json");
        }

        [Fact]
        public void build_with_multiple_mimetypes()
        {
            var list = new MimeTypeList("text/json,application/json");
            list.ShouldHaveTheSameElementsAs("text/json", "application/json");
        }

        [Fact]
        public void  build_with_complex_mimetypes()
        {
            var list =
                new MimeTypeList(
                    "application/xml,application/xhtml+xml,text/html;q=0.9, text/plain;q=0.8,image/png,*/*;q=0.5");

            list.ShouldHaveTheSameElementsAs("application/xml", "application/xhtml+xml", "text/html", "text/plain", "image/png", "*/*");
        }

        [Fact]
        public void matches_positive()
        {
            var list = new MimeTypeList("text/json,application/json");
            list.Matches("text/json").ShouldBeTrue();
            list.Matches("application/json").ShouldBeTrue();
            list.Matches("text/json", "application/json").ShouldBeTrue();
        }

        [Fact]
        public void matches_negative()
        {
            var list = new MimeTypeList("text/json,application/json");
            list.Matches("weird").ShouldBeFalse();
            list.Matches("weird", "wrong").ShouldBeFalse();
        }

        [Fact]
        public void should_ignore_null()
        {
            var list = new MimeTypeList((string)null);
            list.ShouldHaveCount(0);
        }

        [Fact]
        public void should_ignore_empty_string()
        {
            var list = new MimeTypeList(string.Empty);
            list.ShouldHaveCount(0);
        }

        [Fact]
        public void should_ignore_whitespace_only_string()
        {
            var list = new MimeTypeList("    ");
            list.ShouldHaveCount(0);
        }
    }
}