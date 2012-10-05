using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Files
{    
    [TestFixture]
    public class when_finding_files_in_the_asset_pipeline
    {
        private AssetFileGraph _theFileGraph;
        private AssetFile _theFile;
        private AssetPath _thePath;

        [SetUp]
        public void SetUp()
        {
            _theFileGraph = new AssetFileGraph();
            _theFile = new AssetFile("a.js");
            _thePath = new AssetPath("pak1", "a.js", AssetFolder.scripts);
            _theFileGraph.AddFile(_thePath, _theFile);
        }

        [Test]
        public void should_use_case_insensitive_search_when_searching_by_file_only()
        {            
            var assetFile = _theFileGraph.Find("A.js");
            assetFile.ShouldNotBeNull();
            assetFile.Name.ShouldEqual("a.js");            
        }     
    }

    [TestFixture]
    public class AssetFileGraphTester
    {
        private AssetFileGraph _theFileGraph;
        private AssetFileDataMother theFiles;

        [SetUp]
        public void SetUp()
        {
            _theFileGraph = new AssetFileGraph();
            theFiles = new AssetFileDataMother(_theFileGraph.AddFile);
        }

        [Test]
        public void adding_a_file_by_path_sets_the_folder_on_the_file()
        {
            // This is important for later
            var theFile = new AssetFile("a.js");

            var thePath = new AssetPath("pak1", "a.js", AssetFolder.styles);

            _theFileGraph.AddFile(thePath, theFile);

            theFile.Folder.ShouldEqual(thePath.Folder);
        }

        [Test]
        public void find_exact_match_with_package_type_and_name()
        {
            theFiles.LoadAssets(@"
app=application:scripts/jquery.js
pak1-image=pak1:images/jquery.js
pak1-script=pak1:scripts/jquery.js
pak2=pak1:scripts/jquery.js
");

            _theFileGraph.Find("pak1:scripts/jquery.js").ShouldBeTheSameAs(theFiles["pak1-script"]);
        }

        [Test]
        public void application_takes_precedence_over_package_asset_without_any_override()
        {
            theFiles.LoadAssets(@"
app=application:scripts/jquery.js
pak1=pak1:scripts/jquery.js
");

            _theFileGraph.Find("scripts/jquery.js").ShouldBeTheSameAs(theFiles["app"]);
        }

        [Test]
        public void package_order_takes_precedence_without_any_overrides()
        {
            theFiles.LoadAssets(@"
pak1=pak1:scripts/jquery.js
pak2=pak2:scripts/jquery.js
pak3=pak3:scripts/jquery.js
");

            _theFileGraph.Find("scripts/jquery.js").ShouldBeTheSameAs(theFiles["pak1"]);

        }

        [Test]
        public void overrides_can_override_application_precedence()
        {
            theFiles.LoadAssets(@"
app=application:scripts/jquery.js
pak1=pak1:scripts/jquery.js!override
");

            _theFileGraph.Find("scripts/jquery.js").ShouldBeTheSameAs(theFiles["pak1"]);
        }
    }
}