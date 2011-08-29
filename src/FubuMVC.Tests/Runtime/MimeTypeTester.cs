using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class MimeTypeTester
    {
        [Test]
        public void javascript_folder_is_scripts()
        {
            MimeType.Javascript.Folder().ShouldEqual(AssetFolder.scripts);
        }

        [Test]
        public void css_folder_is_styles()
        {
            MimeType.Css.Folder().ShouldEqual(AssetFolder.styles);
        }

        [Test]
        public void image_types_should_be_in_folder_images()
        {
            MimeType.Gif.Folder().ShouldEqual(AssetFolder.images);
            MimeType.Png.Folder().ShouldEqual(AssetFolder.images);
            MimeType.Bmp.Folder().ShouldEqual(AssetFolder.images);
            MimeType.Jpg.Folder().ShouldEqual(AssetFolder.images);
        }

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
        public void determine_mime_type_for_an_extension_that_has_been_added()
        {
            MimeType.Javascript.AddExtension(".coffee");
            MimeType.Css.AddExtension(".scss");

            MimeType.MimeTypeByFileName("file.coffee").ShouldEqual(MimeType.Javascript);
            MimeType.MimeTypeByFileName("file.scss").ShouldEqual(MimeType.Css);
        }
    }
}