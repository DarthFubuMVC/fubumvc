using System;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class AssetFileGraphBuilderIntegratedTester
    {
        private FileSystem system;
        private AssetFileGraph _theFileGraph;
        private AssetFileGraphBuilder theBuilder;
        private const string AppDirectory = "application";
        private const string PackageDirectory = "pak1";

        [SetUp]
        public void SetUp()
        {
            system = new FileSystem();
            system.DeleteDirectory(AppDirectory);
            system.DeleteDirectory(PackageDirectory);

            writeFiles();


            _theFileGraph = new AssetFileGraph();
            theBuilder = new AssetFileGraphBuilder(new FileSystem(), _theFileGraph, new PackageLog());
            theBuilder.LoadFiles(new PackageAssetDirectory(){
                Directory = AppDirectory,
                PackageName = AssetFileGraph.Application
            });

            theBuilder.LoadFiles(new PackageAssetDirectory(){
                Directory = PackageDirectory,
                PackageName = "pak1"
            });
        }

        private void writeFiles()
        {
            writeAppFile(AssetFolder.scripts, "jquery.js");
            writeAppFile(AssetFolder.scripts, "jquery.forms.js");
            writeAppFile(AssetFolder.scripts, "folder1", "script1.js");
            writeAppFile(AssetFolder.scripts, "folder1", "script2.js");
            writeAppFile(AssetFolder.scripts, "folder1", "folder2", "script3.js");

            writeAppFile(AssetFolder.styles, "main.css");
            writeAppFile(AssetFolder.styles, "sidebar.css");
            writeAppFile(AssetFolder.styles, "folder2", "page.css");

            writePackageFile(AssetFolder.scripts, "jquery.js");
            writePackageFile(AssetFolder.scripts, "pak1.js");

            writePackageFile(AssetFolder.scripts, "overrides", "folder1", "script1.js");
        }

        private void writeAppFile(AssetFolder assetFolder, params string[] names)
        {
            var path = AppDirectory.ToFullPath().AppendPath("content", assetFolder.ToString()).AppendPath(names);
            system.WriteStringToFile(path, "something");
        }

        private void writePackageFile(AssetFolder assetFolder, params string[] names)
        {
            var path = PackageDirectory.ToFullPath().AppendPath("content", assetFolder.ToString()).AppendPath(names);
            system.WriteStringToFile(path, "something");
        }

        [Test]
        public void find_asset_file_by_path()
        {
            var path = AppDirectory.ToFullPath().AppendPath("content", "scripts", "folder1", "script1.js");
            var file = _theFileGraph.FindByPath(path);

            file.FullPath.ShouldEqual(path);
            file.Name.ShouldEqual("folder1/script1.js");
        }

        [Test]
        public void load_asset_files_from_an_overrides_sub_folder()
        {
            var file = _theFileGraph.Find("folder1/script1.js");
            file.Override.ShouldBeTrue();
            file.FullPath.ShouldContain(FileSystem.Combine("pak1","content","scripts", "overrides","folder1", "script1.js"));
        }

        [Test]
        public void can_fetch_files()
        {
            _theFileGraph.Find("jquery.js").FullPath.ShouldContain(AppDirectory);
            _theFileGraph.Find("pak1:jquery.js").FullPath.ShouldContain(PackageDirectory);
        }

        [Test]
        public void verify_the_styles()
        {
            var styles = _theFileGraph.AssetsFor(AssetFileGraph.Application).FilesForAssetType(AssetFolder.styles);
            styles.OrderBy(x => x.Name).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("folder2/page.css", "main.css", "sidebar.css");
        }

        [Test]
        public void verify_that_application_files_were_loaded()
        {
            var scripts = _theFileGraph.AssetsFor(AssetFileGraph.Application).FilesForAssetType(AssetFolder.scripts);
            scripts.OrderBy(x => x.Name).Select(x => x.Name).ShouldHaveTheSameElementsAs(
                "folder1/folder2/script3.js",
                "folder1/script1.js",
                "folder1/script2.js",
                "jquery.forms.js",
                "jquery.js"
                );

        }

        [Test]
        public void verify_the_file_path_is_correct()
        {
            var scripts = _theFileGraph.AssetsFor(AssetFileGraph.Application).FilesForAssetType(AssetFolder.scripts);

            var file = scripts.Single(x => x.Name == "folder1/script1.js");
            file.FullPath.ShouldEqual(AppDirectory.AppendPath("content", AssetFolder.scripts.ToString(), "folder1", "script1.js").ToFullPath());   
        }


    }
}