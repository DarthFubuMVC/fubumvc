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
        public void determine_mime_type_from_name_for_js()
        {
            MimeType.DetermineMimeTypeFromName("file.coffee.js")
                .ShouldEqual(MimeType.Javascript);
        }

        [Test]
        public void determine_mime_type_from_name_for_css()
        {
            MimeType.DetermineMimeTypeFromName("style.css")
                .ShouldEqual(MimeType.Css);
        }

        [Test]
        public void determine_mime_type_for_an_extension_that_has_been_added()
        {
            MimeType.Javascript.AddExtension(".coffee");
            MimeType.Css.AddExtension(".scss");

            MimeType.DetermineMimeTypeFromName("file.coffee").ShouldEqual(MimeType.Javascript);
            MimeType.DetermineMimeTypeFromName("file.scss").ShouldEqual(MimeType.Css);
        }
    }
}