using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class ImageExpressionTester
    {
        [Test]
        public void image_tag()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();
            var urls = new StubUrlRegistry();
            page.Stub(x => x.Urls).Return(urls);

            page.Image("some icon name").Attr("src")
                .ShouldEqual(urls.UrlForAsset(AssetFolder.images, "some icon name"));
        }

        [Test]
        public void image_url()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();
            var urls = new StubUrlRegistry();
            page.Stub(x => x.Urls).Return(urls);

            page.ImageUrl("some icon name")
                .ShouldEqual(urls.UrlForAsset(AssetFolder.images, "some icon name"));
        }
    }
}