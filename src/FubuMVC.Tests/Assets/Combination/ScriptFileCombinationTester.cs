using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class ScriptFileCombinationTester
    {
        [Test]
        public void mime_type_has_to_be_javascript()
        {
            var combination = new ScriptFileCombination(new AssetFile[0]);
            combination.MimeType.ShouldEqual(MimeTypeProvider.JAVASCRIPT);
        }

        [Test]
        public void asset_folder_has_to_be_scripts()
        {
            var combination = new ScriptFileCombination(new AssetFile[0]);
            combination.Folder.ShouldEqual(AssetFolder.scripts);
        }
    }
}