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

            var provider = new DefaultMimeTypeProvider();

            scriptFile.DetermineMimetype(provider);
            scriptFile.MimeType.ShouldEqual(DefaultMimeTypeProvider.JAVASCRIPT);

            cssFile.DetermineMimetype(provider);
            cssFile.MimeType.ShouldEqual(DefaultMimeTypeProvider.CSS);
        }

        [Test]
        public void unknown_mimetype_is_thrown_if_the_mimetype_cannot_be_determined()
        {
            var weirdFile = new AssetFile{
                Name = "file1.weird"
            };

            Exception<UnknownExtensionException>.ShouldBeThrownBy(() =>
            {
                weirdFile.DetermineMimetype(new DefaultMimeTypeProvider());
            }).Message.ShouldContain(".weird");
        }
    }
}