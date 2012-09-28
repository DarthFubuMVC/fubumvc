using FubuHtml;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;
using FubuMVC.Core.UI;
using FubuTestingSupport;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class ImageForExtensionMethodTester
    {
        [Test]
        public void image_for()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();
            var urls = MockRepository.GenerateMock<IUrlRegistry>();

            page.Stub(x => x.Urls).Return(urls);

            var assetName = "icon.png";
            var url = "the url for icon.png";

            urls.Stub(x => x.UrlForAsset(AssetFolder.images, assetName))
                .Return(url);

            page.ImageFor(assetName).ToString().ShouldEqual("<img src=\"the url for icon.png\" />");
        }
    }
}