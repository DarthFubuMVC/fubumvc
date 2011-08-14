using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class StyleFileCombinationTester
    {
        [Test]
        public void mime_type_has_to_be_css()
        {
            var combination = new StyleFileCombination(null, new AssetFile[0]);
            combination.MimeType.ShouldEqual(MimeTypeProvider.CSS);
        }

        [Test]
        public void asset_folder_has_to_be_styles()
        {
            var combination = new StyleFileCombination(null, new AssetFile[0]);
            combination.Folder.ShouldEqual(AssetFolder.styles);
        }
    }
}