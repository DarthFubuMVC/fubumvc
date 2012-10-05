using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.Testing
{
    [TestFixture]
    public class ImageForExtensionMethodTester
    {
        [Test]
        public void image_for()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();
            var urls = MockRepository.GenerateMock<IAssetUrls>();

            page.Stub(x => x.Get<IAssetUrls>()).Return(urls);

            var assetName = "icon.png";
            var url = "the url for icon.png";

            urls.Stub(x => x.UrlForAsset(AssetFolder.images, assetName))
                .Return(url);

            page.ImageFor(assetName).ToString().ShouldEqual("<img src=\"the url for icon.png\" />");
        }
    }
}