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
    public class AssetPipelineBuilderIntegratedTester
    {
        private FileSystem system;
        private AssetPipeline thePipeline;
        private AssetPipelineBuilder theBuilder;
        private const string AppDirectory = "application";
        private const string PackageDirectory = "pak1";

        [SetUp]
        public void SetUp()
        {
            system = new FileSystem();
            system.DeleteDirectory(AppDirectory);
            system.DeleteDirectory(PackageDirectory);

            writeFiles();


            thePipeline = new AssetPipeline();
            theBuilder = new AssetPipelineBuilder(new FileSystem(), thePipeline, new PackageLog());
            theBuilder.LoadFiles(new PackageAssetDirectory(){
                Directory = AppDirectory,
                PackageName = AssetPipeline.Application
            });

            theBuilder.LoadFiles(new PackageAssetDirectory(){
                Directory = PackageDirectory,
                PackageName = "pak1"
            });
        }

        private void writeFiles()
        {
            writeAppFile(AssetType.scripts, "jquery.js");
            writeAppFile(AssetType.scripts, "jquery.forms.js");
            writeAppFile(AssetType.scripts, "folder1", "script1.js");
            writeAppFile(AssetType.scripts, "folder1", "script2.js");
            writeAppFile(AssetType.scripts, "folder1", "folder2", "script3.js");

            writeAppFile(AssetType.styles, "main.css");
            writeAppFile(AssetType.styles, "sidebar.css");
            writeAppFile(AssetType.styles, "folder2", "page.css");

            writePackageFile(AssetType.scripts, "jquery.js");
            writePackageFile(AssetType.scripts, "pak1.js");
        }

        private void writeAppFile(AssetType assetType, params string[] names)
        {
            var path = AppDirectory.ToFullPath().AppendPath("content", assetType.ToString()).AppendPath(names);
            system.WriteStringToFile(path, "something");
        }

        private void writePackageFile(AssetType assetType, params string[] names)
        {
            var path = PackageDirectory.ToFullPath().AppendPath("content", assetType.ToString()).AppendPath(names);
            system.WriteStringToFile(path, "something");
        }

        [Test]
        public void can_fetch_files()
        {
            thePipeline.Find("jquery.js").FullPath.ShouldContain(AppDirectory);
            thePipeline.Find("pak1:jquery.js").FullPath.ShouldContain(PackageDirectory);
        }

        [Test]
        public void verify_the_styles()
        {
            var styles = thePipeline.AssetsFor(AssetPipeline.Application).FilesForAssetType(AssetType.styles);
            styles.OrderBy(x => x.Name).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("folder2/page.css", "main.css", "sidebar.css");
        }

        [Test]
        public void verify_that_application_files_were_loaded()
        {
            var scripts = thePipeline.AssetsFor(AssetPipeline.Application).FilesForAssetType(AssetType.scripts);
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
            var scripts = thePipeline.AssetsFor(AssetPipeline.Application).FilesForAssetType(AssetType.scripts);

            var file = scripts.Single(x => x.Name == "folder1/script1.js");
            file.FullPath.ShouldEqual(AppDirectory.AppendPath("content", AssetType.scripts.ToString(), "folder1", "script1.js").ToFullPath());   
        }
    }
}