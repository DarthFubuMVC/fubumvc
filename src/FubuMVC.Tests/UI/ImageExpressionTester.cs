using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.UI;
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

            page.Image("some icon name").Attr("src").ShouldEqual(
                AssetContentHandler.DetermineAssetUrl(AssetFolder.images, "some icon name"));
        }

        [Test]
        public void image_url()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();

            page.ImageUrl("some icon name").ShouldEqual(AssetContentHandler.DetermineAssetUrl(AssetFolder.images,
                                                                                                  "some icon name"));
        }
    }
}