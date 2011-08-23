using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetFileHandlerTester
    {
        [SetUp]
        public void SetUp()
        {
            UrlContext.Stub("http://myapp.com");
        }

        [TearDown]
        public void Teardown()
        {
            UrlContext.Stub();
        }


        [Test]
        public void determine_asset_url_simple()
        {
            var file = new AssetFile{
                Folder = AssetFolder.scripts,
                Name = "jquery.forms.js"
            };

            AssetFileHandler.DetermineAssetUrl(file)
                .ShouldEqual("http://myapp.com/_content/scripts/jquery.forms.js");
        }

        [Test]
        public void determine_asset_url_complex()
        {
            var file = new AssetFile
            {
                Folder = AssetFolder.scripts,
                Name = "shared/jquery.forms.js"
            };

            AssetFileHandler.DetermineAssetUrl(file)
                .ShouldEqual("http://myapp.com/_content/scripts/shared/jquery.forms.js"); 
        }
    }
}