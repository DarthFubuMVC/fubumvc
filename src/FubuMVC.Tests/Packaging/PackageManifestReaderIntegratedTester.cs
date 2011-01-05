using System;
using System.IO;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using FubuCore;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class PackageManifestReaderIntegratedTester
    {
        private string packageFolder;
        private PackageManifestReader reader;

        [SetUp]
        public void SetUp()
        {
            packageFolder = FileSystem.Combine("../../../TestPackage1").ToFullPath();

            var fileSystem = new FileSystem();
            var manifest = new PackageManifest(){
                Assemblies = "TestPackage1",
                Name = "pak1"
            };

            fileSystem.PersistToFile(manifest, packageFolder, PackageManifest.FILE);

            reader = new PackageManifestReader("../../".ToFullPath(), fileSystem, folder => folder);
        }



        [TearDown]
        public void TearDown()
        {
            new FileSystem().DeleteFile(FileSystem.Combine("../../".ToFullPath(), ApplicationManifest.FILE));
        }



        [Test]
        public void load_a_package_info_from_a_manifest_file_when_given_the_folder()
        {
            // the reader is rooted at the folder location of the main app
            var package = reader.LoadFromFolder("../../../TestPackage1".ToFullPath());

            var assemblyLoader = new AssemblyLoader(new PackagingDiagnostics());
            assemblyLoader.AssemblyFileLoader = file => Assembly.Load(File.ReadAllBytes(file));
            assemblyLoader.LoadAssembliesFromPackage(package);

            var loadedAssemblies = assemblyLoader.Assemblies.ToArray();
            loadedAssemblies.ShouldHaveCount(1);
            loadedAssemblies[0].GetName().Name.ShouldEqual("TestPackage1");
        }

        [Test]
        public void load_a_package_registers_web_content_folder()
        {
            var packageDirectory = "../../../TestPackage1".ToFullPath();
            var package = reader.LoadFromFolder(packageDirectory);
            var directoryContinuation = MockRepository.GenerateMock<Action<string>>();
        
            package.ForFolder(FubuMvcPackages.WebContentFolder, directoryContinuation);
        
            directoryContinuation.AssertWasCalled(x => x.Invoke(packageDirectory));
        }

        [Test]
        public void load_all_packages_by_reading_the_include_folder()
        {
            var includes = new ApplicationManifest();
            includes.AddLink("../TestPackage1");

            new FileSystem().PersistToFile(includes, "../../".ToFullPath(), ApplicationManifest.FILE);

            var assemblyLoader = new AssemblyLoader(new PackagingDiagnostics());
            assemblyLoader.AssemblyFileLoader = file => Assembly.Load(File.ReadAllBytes(file));

            var package = reader.Load().Single();
            assemblyLoader.LoadAssembliesFromPackage(package);

            assemblyLoader.Assemblies.Single().GetName().Name.ShouldEqual("TestPackage1");
        }
    }
}