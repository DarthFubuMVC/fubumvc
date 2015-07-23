using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class MimeTypeTester
    {
        [Test]
        public void find_default_extension_for_javascript()
        {
            MimeType.Javascript.DefaultExtension().ShouldBe(".js");
        }

        [Test]
        public void find_default_extension_for_css()
        {
            MimeType.Css.DefaultExtension().ShouldBe(".css");
        }

        [Test]
        public void find_default_extension_for_truetype_font()
        {
            MimeType.TrueTypeFont.DefaultExtension().ShouldBe(".ttf");
        }

        [Test]
        public void determine_mime_type_from_name_for_js()
        {
            MimeType.MimeTypeByFileName("file.coffee.js")
                .ShouldBe(MimeType.Javascript);
        }

        [Test]
        public void determine_mime_type_from_name_for_css()
        {
            MimeType.MimeTypeByFileName("style.css")
                .ShouldBe(MimeType.Css);
        }

        [Test]
        public void determine_mime_type_from_name_for_truetype_font()
        {
            MimeType.MimeTypeByFileName("somefont.ttf")
                .ShouldBe(MimeType.TrueTypeFont);
        }

        [Test]
        public void determine_mime_type_for_an_extension_that_has_been_added()
        {
            MimeType.Javascript.AddExtension(".coffee");
            MimeType.Css.AddExtension(".scss");

            MimeType.MimeTypeByFileName("file.coffee").ShouldBe(MimeType.Javascript);
            MimeType.MimeTypeByFileName("file.scss").ShouldBe(MimeType.Css);
        }

        [Test]
        public void can_return_null_for_a_totally_unrecognized_extension()
        {
            MimeType.MimeTypeByFileName("foo.657878XXXXXX")
                .ShouldBeNull();
        }

        [Test]
        public void mimetype_from_extended_extension_set()
        {
            MimeType.MimeTypeByFileName("foo.323")
                .Value.ShouldBe("text/h323");
        }
    }
}