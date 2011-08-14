using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Content;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class AssetFileTester
    {
        [Test]
        public void finds_the_extension_of_itself()
        {
            var file = new AssetFile{
                Name = "script.js"
            };

            file.Extension().ShouldEqual(".js");
        }

        [Test]
        public void finds_the_extension_even_with_multiple_dots()
        {
            var file = new AssetFile(){
                Name = "jquery.forms.js"
            };

            file.Extension().ShouldEqual(".js");
        }

        [Test]
        public void use_the_asset_folder_while_determining_mimetype()
        {
            var coffee = new AssetFile(){
                Name = "something.coffee", 
                Folder = AssetFolder.scripts
            };

            var provider = new MimeTypeProvider();
            coffee.DetermineMimetype(provider);

            coffee.MimeType.ShouldEqual(MimeTypeProvider.JAVASCRIPT);
        }

        [Test]
        public void determine_mimetype_positive()
        {
            var scriptFile = new AssetFile
            {
                Name = "script.js"
            };

            var cssFile = new AssetFile
            {
                Name = "main.css"
            };

            var provider = new MimeTypeProvider();

            scriptFile.DetermineMimetype(provider);
            scriptFile.MimeType.ShouldEqual(MimeTypeProvider.JAVASCRIPT);

            cssFile.DetermineMimetype(provider);
            cssFile.MimeType.ShouldEqual(MimeTypeProvider.CSS);
        }

    }
}