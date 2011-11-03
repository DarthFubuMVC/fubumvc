using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class PackageAssetsTester
    {
        private PackageAssets thePackageAssets;
        private AssetFileDataMother theFiles;

        [SetUp]
        public void SetUp()
        {
            thePackageAssets = new PackageAssets("pak1");
            theFiles = new AssetFileDataMother(thePackageAssets.AddFile);
        }

        [Test]
        public void find_by_unique_name_with_no_duplicates_can_look_through_all_types()
        {
            theFiles.LoadAssets(@"
jquery=scripts/jquery.js
icon=images/icon.gif
main=styles/main.css
");

            thePackageAssets.FindByName("jquery.js").ShouldBeTheSameAs(theFiles["jquery"]);
            thePackageAssets.FindByName("icon.gif").ShouldBeTheSameAs(theFiles["icon"]);
            thePackageAssets.FindByName("main.css").ShouldBeTheSameAs(theFiles["main"]);
        }

        [Test]
        public void find_by_unique_name_with_duplicates_should_look_in_scripts_then_styles_then_images()
        {
            theFiles.LoadAssets(@"
script-jquery=scripts/jquery.js
images-jquery=images/jquery.js
styles-jquery=styles/jquery.js
styles-main=styles/main.css
images-main=images/main.css
");

            thePackageAssets.FindByName("jquery.js").ShouldBeTheSameAs(theFiles["script-jquery"]);
            thePackageAssets.FindByName("main.css").ShouldBeTheSameAs(theFiles["styles-main"]);
        }

        [Test]
        public void find_by_type_and_name()
        {
            theFiles.LoadAssets(@"
script-jquery=scripts/jquery.js
images-jquery=images/jquery.js
styles-jquery=styles/jquery.js
styles-main=styles/main.css
images-main=images/main.css
");

            thePackageAssets.FindByName("scripts/jquery.js").ShouldBeTheSameAs(theFiles["script-jquery"]);
            thePackageAssets.FindByName("styles/jquery.js").ShouldBeTheSameAs(theFiles["styles-jquery"]);
            thePackageAssets.FindByName("images/jquery.js").ShouldBeTheSameAs(theFiles["images-jquery"]);
        }

        [Test]
        public void no_match_should_be_null()
        {
            theFiles.LoadAssets(@"
script-jquery=scripts/jquery.js
images-jquery=images/jquery.js
styles-jquery=styles/jquery.js
styles-main=styles/main.css
images-main=images/main.css
");

            thePackageAssets.FindByName("nothing/that/exists").ShouldBeNull();
        }


    }
}