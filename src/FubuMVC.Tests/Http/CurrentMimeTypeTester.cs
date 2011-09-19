using FubuMVC.Core.Http;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class CurrentMimeTypeTester
    {
        [Test]
        public void requested_mime_type()
        {
            
        }
    }

    [TestFixture]
    public class MimeTypeListTester
    {
        [Test]
        public void build_from_string()
        {
            var list = new MimeTypeList("text/json");
            list.ShouldHaveTheSameElementsAs("text/json");
        }

        [Test]
        public void build_with_multiple_mimetypes()
        {
            var list = new MimeTypeList("text/json,application/json");
            list.ShouldHaveTheSameElementsAs("text/json", "application/json");
        }

        [Test]
        public void  build_with_complex_mimetypes()
        {
            var list =
                new MimeTypeList(
                    "application/xml,application/xhtml+xml,text/html;q=0.9, text/plain;q=0.8,image/png,*/*;q=0.5");

            list.ShouldHaveTheSameElementsAs("application/xml", "application/xhtml+xml", "text/html", "text/plain", "image/png", "*/*");
        }

        [Test]
        public void matches_positive()
        {
            var list = new MimeTypeList("text/json,application/json");
            list.Matches("text/json").ShouldBeTrue();
            list.Matches("application/json").ShouldBeTrue();
            list.Matches("text/json", "application/json").ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            var list = new MimeTypeList("text/json,application/json");
            list.Matches("weird").ShouldBeFalse();
            list.Matches("weird", "wrong").ShouldBeFalse();
        }
    }
}