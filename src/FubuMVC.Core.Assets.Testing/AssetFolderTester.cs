using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.Testing
{
    [TestFixture]
    public class AssetFolderTester
    {
        [Test]
        public void javascript_folder_is_scripts()
        {
            AssetFolder.FolderFor(MimeType.Javascript).ShouldEqual(AssetFolder.scripts);
        }

        [Test]
        public void css_folder_is_styles()
        {
            AssetFolder.FolderFor(MimeType.Css).ShouldEqual(AssetFolder.styles);
        }

        [Test]
        public void image_types_should_be_in_folder_images()
        {
            AssetFolder.FolderFor(MimeType.Gif).ShouldEqual(AssetFolder.images);
            AssetFolder.FolderFor(MimeType.Png).ShouldEqual(AssetFolder.images);
            AssetFolder.FolderFor(MimeType.Bmp).ShouldEqual(AssetFolder.images);
            AssetFolder.FolderFor(MimeType.Jpg).ShouldEqual(AssetFolder.images);
        }

        [Test]
        public void truetype_fonts_recorded_as_images_to_force_binary()
        {
            AssetFolder.FolderFor(MimeType.TrueTypeFont).ShouldEqual(AssetFolder.fonts);
        }
    }
}