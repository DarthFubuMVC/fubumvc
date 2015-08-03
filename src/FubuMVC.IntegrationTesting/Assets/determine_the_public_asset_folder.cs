using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.View;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class determine_the_public_asset_folder
    {

        [Test]
        public void public_folder_only()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AssetSettings>(x =>
            {
                x.Mode = SearchMode.PublicFolderOnly;
                x.Version = null;
            });

            using (var runtime = registry.ToRuntime())
            {
                runtime.Scenario(_ =>
                {
                    _.Get.Action<PublicAssetFolderEndpoint>(x => x.get_public_asset_folder());

                    _.ContentShouldContain("*/public*");
                });
            }
        }

        [Test]
        public void public_folder_and_version()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AssetSettings>(x =>
            {
                x.Mode = SearchMode.PublicFolderOnly;
                x.Version = "1.0.1";
            });

            using (var runtime = registry.ToRuntime())
            {
                runtime.Scenario(_ =>
                {
                    _.Get.Action<PublicAssetFolderEndpoint>(x => x.get_public_asset_folder());

                    _.ContentShouldContain("*/public/1.0.1*");
                });
            }
        }
    }

    public class PublicAssetFolderEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public PublicAssetFolderEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public HtmlDocument get_public_asset_folder()
        {
            var folder = _document.PublicAssetFolder();
            _document.Add("p").Text("*" + folder + "*");

            return _document;
        }
    }
}