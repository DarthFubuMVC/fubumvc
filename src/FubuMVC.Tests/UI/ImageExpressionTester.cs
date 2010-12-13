using FubuMVC.Core.Content;
using FubuMVC.Core.View;
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
            var registry = MockRepository.GenerateMock<IContentRegistry>();
            var page = MockRepository.GenerateMock<IFubuPage>();

            page.Stub(x => x.Get<IContentRegistry>()).Return(registry);

            registry.Stub(x => x.ImageUrl("some icon name")).Return("some url");

            page.ImageUrl("some icon name").ShouldEqual("some url");
        }

        [Test]
        public void image_tag()
        {
            var registry = MockRepository.GenerateMock<IContentRegistry>();
            var page = MockRepository.GenerateMock<IFubuPage>();

            page.Stub(x => x.Get<IContentRegistry>()).Return(registry);

            registry.Stub(x => x.ImageUrl("some icon name")).Return("some url");

            page.Image("some icon name").Attr("src").ShouldEqual("some url");
        }
    }
}