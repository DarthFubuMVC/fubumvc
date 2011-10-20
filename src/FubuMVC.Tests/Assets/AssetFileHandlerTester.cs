using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetFileHandlerTester
    {


        [Test]
        public void determine_asset_url_simple()
        {
            var file = new AssetFile("jquery.forms.js")
            {
                Folder = AssetFolder.scripts
            };

            AssetContentHandler.DetermineAssetUrl(file)
                .ShouldEqual("http://myapp.com/_content/scripts/jquery.forms.js");
        }

        [Test]
        public void determine_asset_url_complex()
        {
            var file = new AssetFile("shared/jquery.forms.js")
            {
                Folder = AssetFolder.scripts
            };

            AssetContentHandler.DetermineAssetUrl(file)
                .ShouldEqual("http://myapp.com/_content/scripts/shared/jquery.forms.js"); 
        }
    }
}