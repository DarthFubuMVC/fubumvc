using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class MimeTypeTester
    {
        [Test]
        public void find_default_extension_for_javascript()
        {
            MimeType.Javascript.DefaultExtension().ShouldEqual(".js");
        }

        [Test]
        public void find_default_extension_for_css()
        {
            MimeType.Css.DefaultExtension().ShouldEqual(".css");
        }

        [Test]
        public void find_default_extension_for_truetype_font()
        {
            MimeType.TrueTypeFont.DefaultExtension().ShouldEqual(".ttf");
        }

        [Test]
        public void determine_mime_type_from_name_for_js()
        {
            MimeType.MimeTypeByFileName("file.coffee.js")
                .ShouldEqual(MimeType.Javascript);
        }

        [Test]
        public void determine_mime_type_from_name_for_css()
        {
            MimeType.MimeTypeByFileName("style.css")
                .ShouldEqual(MimeType.Css);
        }

        [Test]
        public void determine_mime_type_from_name_for_truetype_font()
        {
            MimeType.MimeTypeByFileName("somefont.ttf")
                .ShouldEqual(MimeType.TrueTypeFont);
        }

        [Test]
        public void determine_mime_type_for_an_extension_that_has_been_added()
        {
            MimeType.Javascript.AddExtension(".coffee");
            MimeType.Css.AddExtension(".scss");

            MimeType.MimeTypeByFileName("file.coffee").ShouldEqual(MimeType.Javascript);
            MimeType.MimeTypeByFileName("file.scss").ShouldEqual(MimeType.Css);
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
                .Value.ShouldEqual("text/h323");
        }
    }
}