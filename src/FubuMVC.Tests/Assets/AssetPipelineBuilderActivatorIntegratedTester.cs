using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Tests.Content;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetPipelineBuilderActivatorIntegratedTester
    {
        private string currentDirectory;
        private string packageDirectory;
        private IFileSystem fileSystem = new FileSystem();
        private string _packageName;
        private System.Collections.Generic.List<IPackageInfo> _packages;
        private AssetPipeline thePipeline;
        private PackageLog theLog;

        private void startPackage(string packageName)
        {
            fileSystem.DeleteDirectory(packageName);
            _packageName = packageName;

            var directory = ".".AppendPath(packageName);
            var package = new StubPackage(packageName);
            package.RegisterFolder(BottleFiles.WebContentFolder, directory);
            _packages.Add(package);

            packageDirectory = directory;
        }

        private AssetType AssetTypeIs
        {
            set
            {
                currentDirectory = packageDirectory.AppendPath("content", value.ToString());
            }
        }

        private void writeFile(string name)
        {
            var file = currentDirectory.AppendPath(name.Replace('/', Path.DirectorySeparatorChar));
            fileSystem.WriteStringToFile(file, "something");
        }

        [SetUp]
        public void SetUp()
        {
            packageDirectory = ".".ToFullPath();
            _packages = new List<IPackageInfo>();

            AssetTypeIs = AssetType.scripts;
            writeFile("a.js");
            writeFile("b.js");
            writeFile("folder1/c.js");
            writeFile("folder2/d.js");

            AssetTypeIs = AssetType.styles;
            writeFile("main.css");
            writeFile("page.css");
            writeFile("folder1/a.css");

            startPackage("pak1");
            AssetTypeIs = AssetType.scripts;
            writeFile("e.js");
            writeFile("f.js");

            startPackage("pak2");
            AssetTypeIs = AssetType.styles;
            writeFile("a.css");
            writeFile("b.css");
            writeFile("c.css");

            startPackage("pak3");
            AssetTypeIs = AssetType.scripts;
            writeFile("a.js");

            thePipeline = new AssetPipeline();
            var activator = new AssetPipelineBuilderActivator(thePipeline);
            theLog = new PackageLog();
            activator.Activate(_packages, theLog);

            Debug.WriteLine(theLog.FullTraceText());
        }

        [Test]
        public void picked_up_application_and_all_possible_packages()
        {
            thePipeline.AllPackages.Select(x => x.PackageName).ShouldHaveTheSameElementsAs(AssetPipeline.Application, "pak1", "pak2", "pak3");
        }

        [Test]
        public void got_files_from_package()
        {
            thePipeline.AssetsFor("pak1").FilesForAssetType(AssetType.scripts).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("e.js", "f.js");
        }

        [Test]
        public void find_file()
        {
            thePipeline.Find("a.js").ShouldBeTheSameAs(
                thePipeline.AssetsFor(AssetPipeline.Application).FindByName("a.js"));
        }
        
    }
}