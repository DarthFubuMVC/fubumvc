using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Content;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using FubuMVC.Core.UI;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class ImageExpressionTester
    {
        [Test]
        public void image_url()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();

            page.ImageUrl("some icon name").ShouldEqual(AssetContentFileHandler.DetermineAssetUrl(AssetFolder.images, "some icon name"));
        }

        [Test]
        public void image_tag()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();

            page.Image("some icon name").Attr("src").ShouldEqual(AssetContentFileHandler.DetermineAssetUrl(AssetFolder.images, "some icon name"));
        }
    }
}