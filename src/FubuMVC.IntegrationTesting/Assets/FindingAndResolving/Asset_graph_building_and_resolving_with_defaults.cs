using System.Linq;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets.FindingAndResolving
{
    [TestFixture]
    public class Asset_graph_building_and_resolving_with_defaults : AssetIntegrationContext
    {
        public Asset_graph_building_and_resolving_with_defaults()
        {
            File("MyLib.js").Write("alert(1);");
            File("Folder1/Lib2.js").Write("alert(2);");

            File("Content/styles/MyStyles.css").Write("/* good styles */");

            File("image.bmp");


            // These 2 are NOT assets
            File("foo.txt");
            File("bar.spark");

            File("image.jpg");
            File("Content/scripts/bottle1.js");
            File("Content/scripts/folder1/bottle1A.js");
            File("bottle2.js");

            // Duplicated from the application, but the app wins
            File("MyLib.js").Write("alert(1);");

            File("BottleB.css");

            // To prove that the one in the app wins
            File("bottle2.js");
            File("image.gif");
        }

        [Test]
        public void all_relative_paths_are_canonical()
        {
            AllAssets.Assets.All(x => !x.File.RelativePath.Contains('\\'))
                .ShouldBeTrue();
        }

        [Test]
        public void finds_js_files()
        {
            AllAssets.Assets.Any(x => x.Url == "MyLib.js").ShouldBeTrue();
            AllAssets.Assets.Any(x => x.Url == "bottle2.js").ShouldBeTrue();
        }

        [Test]
        public void finds_css_files()
        {
            AllAssets.Assets.Any(x => x.Url == "Content/styles/MyStyles.css").ShouldBeTrue();
            AllAssets.Assets.Any(x => x.Url == "BottleB.css").ShouldBeTrue();
        }

        [Test]
        public void finds_image_files()
        {
            AllAssets.Assets.Any(x => x.Url == "image.bmp").ShouldBeTrue();
            AllAssets.Assets.Any(x => x.Url == "image.gif").ShouldBeTrue();
            AllAssets.Assets.Any(x => x.Url == "image.jpg").ShouldBeTrue();
        }


        [Test]
        public void file_construction_in_the_application_root()
        {
            var asset = Assets.FindAsset("MyLib.js");
            asset.ShouldNotBeNull();

            asset.Url.ShouldBe("MyLib.js");
            asset.MimeType.ShouldBeTheSameAs(MimeType.Javascript);
            asset.Filename.ShouldBe("MyLib.js");
        }

        [Test]
        public void file_construction_in_a_child_folder_of_the_application()
        {
            var asset = Assets.FindAsset("Content/styles/MyStyles.css");
            asset.ShouldNotBeNull();

            asset.Url.ShouldBe("Content/styles/MyStyles.css");
            asset.MimeType.ShouldBeTheSameAs(MimeType.Css);
            asset.Filename.ShouldBe("MyStyles.css");
        }

        [Test]
        public void search_by_filename_only()
        {
            Assets.FindAsset("bottle1.js").Url.ShouldBe("Content/scripts/bottle1.js");
            Assets.FindAsset("bottle1.js").Url.ShouldBe("Content/scripts/bottle1.js");
            Assets.FindAsset("bottle1.js").Url.ShouldBe("Content/scripts/bottle1.js");
            Assets.FindAsset("bottle1.js").Url.ShouldBe("Content/scripts/bottle1.js");
        }

        [Test]
        public void search_by_full_path()
        {
            Assets.FindAsset("Content/scripts/bottle1.js")
                .Url.ShouldBe("Content/scripts/bottle1.js");
        }

        [Test]
        public void search_by_part_of_the_path()
        {
            Assets.FindAsset("folder1/bottle1A.js")
                .Url.ShouldBe("Content/scripts/folder1/bottle1A.js");
        }
    }
}