using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class MimeTypeProviderTester
    {
        private MimeTypeProvider theProvider;

        [SetUp]
        public void SetUp()
        {
            theProvider = new MimeTypeProvider();
        }

        [Test]
        public void finding_a_mimetype_for_a_known_extension()
        {
            theProvider.For(".gif").ShouldEqual("image/gif");
            theProvider.For(".css").ShouldEqual(MimeTypeProvider.CSS);
        }

        [Test]
        public void finding_a_mimetype_for_an_unknown_extension_throws_an_error()
        {
            Exception<UnknownExtensionException>.ShouldBeThrownBy(() =>
            {
                theProvider.For(".unknown");
            });
        }

        [Test]
        public void finding_a_mimetype_with_the_asset_folder_when_the_extension_is_known()
        {
            theProvider.For(".jpg", AssetFolder.styles).ShouldEqual("image/jpeg");
        }

        [Test]
        public void use_the_default_mimetype_for_the_asset_folder_scripts_if_the_extension_is_unknown()
        {
            theProvider.For(".coffee", AssetFolder.scripts).ShouldEqual(MimeTypeProvider.JAVASCRIPT);
        }

        [Test]
        public void use_the_default_mimetype_for_the_asset_folder_styles_if_the_extension_is_unknown()
        {
            theProvider.For(".scss", AssetFolder.styles).ShouldEqual(MimeTypeProvider.CSS);
        }

        [Test]
        public void no_default_mimetype_for_images_so_it_will_blow_up()
        {
            Exception<UnknownExtensionException>.ShouldBeThrownBy(() =>
            {
                theProvider.For(".weird", AssetFolder.images);
            });
        }
    }
}