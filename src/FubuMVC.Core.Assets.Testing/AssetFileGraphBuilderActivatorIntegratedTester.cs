using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetFileGraphBuilderActivatorIntegratedTester
    {
        private string currentDirectory;
        private string packageDirectory;
        private IFileSystem fileSystem = new FileSystem();
        private string _packageName;
        private System.Collections.Generic.List<IPackageInfo> _packages;
        private AssetFileGraph _theFileGraph;
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

        private AssetFolder AssetFolderIs
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

            AssetFolderIs = AssetFolder.scripts;
            writeFile("a.js");
            writeFile("b.js");
            writeFile("folder1/c.js");
            writeFile("folder2/d.js");

            AssetFolderIs = AssetFolder.styles;
            writeFile("main.css");
            writeFile("page.css");
            writeFile("folder1/a.css");

            startPackage("pak1");
            AssetFolderIs = AssetFolder.scripts;
            writeFile("e.js");
            writeFile("f.js");

            startPackage("pak2");
            AssetFolderIs = AssetFolder.styles;
            writeFile("a.css");
            writeFile("b.css");
            writeFile("c.css");

            startPackage("pak3");
            AssetFolderIs = AssetFolder.scripts;
            writeFile("a.js");

            _theFileGraph = new AssetFileGraph();
            var activator = new AssetFileGraphBuilderActivator(_theFileGraph,new AssetLogsCache());
            theLog = new PackageLog();
            activator.Activate(_packages, theLog);

            Debug.WriteLine(theLog.FullTraceText());
        }

        [Test]
        public void picked_up_application_and_all_possible_packages()
        {
            _theFileGraph.AllPackages.Select(x => x.PackageName).ShouldHaveTheSameElementsAs(AssetFileGraph.Application, "pak1", "pak2", "pak3");
        }

        [Test]
        public void got_files_from_package()
        {
            _theFileGraph.AssetsFor("pak1").FilesForAssetType(AssetFolder.scripts).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("e.js", "f.js");
        }

        [Test]
        public void find_file()
        {
            _theFileGraph.Find("a.js").ShouldBeTheSameAs(
                _theFileGraph.AssetsFor(AssetFileGraph.Application).FindByName("a.js"));
        }
        
    }
}