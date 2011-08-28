using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Content;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class AssetFileTester
    {
        [Test]
        public void content_folder_is_null_if_under_the_root()
        {
            new AssetFile("something.js").ContentFolder().ShouldBeNull();
        }

        [Test]
        public void content_folder_under_a_single_folder()
        {
            new AssetFile("f1/main.css").ContentFolder().ShouldEqual("f1");
        }

        [Test]
        public void content_folder_under_deep_folders()
        {
            new AssetFile("f1/f2/main.css").ContentFolder().ShouldEqual("f1/f2");
        }

        [Test]
        public void derive_the_asset_folder_from_the_mimetype_by_default()
        {
            new AssetFile("script.js").Folder.ShouldEqual(AssetFolder.scripts);
            new AssetFile("style.css").Folder.ShouldEqual(AssetFolder.styles);
        }

        [Test]
        public void finds_the_extension_of_itself()
        {
            var file = new AssetFile("script.js");
            
            file.Extension().ShouldEqual(".js");
        }

        [Test]
        public void finds_the_extension_even_with_multiple_dots()
        {
            var file = new AssetFile("jquery.forms.js");

            file.Extension().ShouldEqual(".js");
        }

        [Test]
        public void use_the_asset_folder_while_determining_mimetype()
        {
            var coffee = new AssetFile("something.coffee"){
                Folder = AssetFolder.scripts
            };

            coffee.MimeType.ShouldEqual(MimeType.Javascript);
        }

        [Test]
        public void determine_mimetype_positive()
        {
            var scriptFile = new AssetFile("script.js");

            var cssFile = new AssetFile("main.css");

            scriptFile.MimeType.ShouldEqual(MimeType.Javascript);

            cssFile.MimeType.ShouldEqual(MimeType.Css);
        }

        [Test]
        public void find_all_the_possible_extensions()
        {
            new AssetFile("script.js").AllExtensions().ShouldHaveTheSameElementsAs(".js");
            new AssetFile("script.trans1.js").AllExtensions().ShouldHaveTheSameElementsAs(".trans1",".js");
            new AssetFile("script.trans1.trans2.js").AllExtensions().ShouldHaveTheSameElementsAs(".trans1", ".trans2",".js");
            new AssetFile("something").AllExtensions().Any().ShouldBeFalse();
        }

    }
}