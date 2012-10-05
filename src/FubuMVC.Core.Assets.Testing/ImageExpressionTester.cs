using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.Testing
{
    [TestFixture]
    public class ImageExpressionTester
    {
        [Test]
        public void image_tag()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();
            var urls = new AssetUrls(new StandInCurrentHttpRequest());
            page.Stub(x => x.Get<IAssetUrls>()).Return(urls);

            page.Image("some icon name").Attr("src")
                .ShouldEqual(urls.UrlForAsset(AssetFolder.images, "some icon name"));
        }

        [Test]
        public void image_url()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();
            var urls = new AssetUrls(new StandInCurrentHttpRequest());
            page.Stub(x => x.Get<IAssetUrls>()).Return(urls);

            page.ImageUrl("some icon name")
                .ShouldEqual(urls.UrlForAsset(AssetFolder.images, "some icon name"));
        }
    }
}