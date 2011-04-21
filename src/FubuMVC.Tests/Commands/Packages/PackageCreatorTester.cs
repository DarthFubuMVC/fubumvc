using System;
using System.Collections.Generic;
using System.IO;
using Bottles;
using Bottles.Assemblies;
using Bottles.Exploding;
using Bottles.Zipping;
using Fubu.Packages.Creation;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands.Packages
{
    [TestFixture]
    public class when_creating_a_package_for_all_assemblies_found_and_including_pdbs : InteractionContext<PackageCreator>
    {
        private PackageManifest theManifest;
        private AssemblyFiles theAssemblyFiles;
        private CreatePackageInput theInput;
        private StubZipFileService _theZipFileService;
        private string thePackageManifestFileName;
		private string theBaseFolder;
		private string theBinFolder;
		
        protected override void beforeEach()
        {
			theBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "folder1");
			theBinFolder = Path.Combine(theBaseFolder, "bin");	
			
            theManifest = new PackageManifest
            {
                DataFileSet = new FileSet(),
                ContentFileSet = new FileSet()
            };

            theManifest.AddAssembly("A");
            theManifest.AddAssembly("B");
            theManifest.AddAssembly("C");

            theInput = new CreatePackageInput()
            {
                PackageFolder = theBaseFolder,
                ZipFile = Path.Combine(theBaseFolder, "package1.zip"),
                PdbFlag = true
            };

            theAssemblyFiles = new AssemblyFiles()
            {
                Files = new string[] { 
					FileSystem.Combine(theBinFolder, "a.dll"), 
					FileSystem.Combine(theBinFolder, "b.dll"), 
					FileSystem.Combine(theBinFolder, "c.dll") 
				},
                MissingAssemblies = new string[0],
                PdbFiles = new string[] { 
					FileSystem.Combine(theBinFolder, "a.pdb"), 
					FileSystem.Combine(theBinFolder, "b.pdb"), 
					FileSystem.Combine(theBinFolder, "c.pdb")
				},
                Success = true
            };

            MockFor<IAssemblyFileFinder>()
                .Stub(x => x.FindAssemblies(theBinFolder, theManifest.Assemblies))
                .Return(theAssemblyFiles);

            _theZipFileService = new StubZipFileService();
            Services.Inject<IZipFileService>(_theZipFileService);

            thePackageManifestFileName = FileSystem.Combine(theBaseFolder, PackageManifest.FILE);

            ClassUnderTest.CreatePackage(theInput, theManifest);
        }

        [Test]
        public void should_not_log_assemblies_missing()
        {
            MockFor<IPackageLogger>().AssertWasNotCalled(x => x.WriteAssembliesNotFound(theAssemblyFiles, theManifest, theInput));
        }

        [Test]
        public void should_have_written_a_zip_file_to_the_value_from_the_input()
        {
            _theZipFileService.FileName.ShouldEqual(theInput.ZipFile);
        }

        [Test]
        public void should_have_written_each_assembly_to_the_zip_file()
        {
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(Path.Combine(theBinFolder, "a.dll"), "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(Path.Combine(theBinFolder, "b.dll"), "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(Path.Combine(theBinFolder, "c.dll"), "bin"));
        }

        [Test]
        public void should_have_written_each_pdb_to_the_zip_file()
        {
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(Path.Combine(theBinFolder, "a.pdb"), "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(Path.Combine(theBinFolder, "b.pdb"), "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(Path.Combine(theBinFolder, "c.pdb"), "bin"));
        }

        [Test]
        public void should_write_the_package_manifest_to_the_zip()
        {
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry(thePackageManifestFileName, string.Empty));
        }

        [Test]
        public void add_the_content_files()
        {
            _theZipFileService.ZipRequests.ShouldContain(new ZipFolderRequest(){
                FileSet = theManifest.ContentFileSet,
                ZipDirectory = BottleFiles.WebContentFolder,
                RootDirectory = theInput.PackageFolder
            });
        }

        [Test]
        public void add_the_data_files()
        {
            _theZipFileService.ZipRequests.ShouldContain(new ZipFolderRequest()
            {
                FileSet = theManifest.DataFileSet,
                ZipDirectory = BottleFiles.DataFolder,
                RootDirectory = Path.Combine(theInput.PackageFolder, BottleFiles.DataFolder)
            });
        }
    }

    [TestFixture]
    public class when_creating_a_package_for_all_assemblies_found_and_not_including_pdbs : InteractionContext<PackageCreator>
    {
        private PackageManifest theManifest;
        private AssemblyFiles theAssemblyFiles;
        private CreatePackageInput theInput;
        private StubZipFileService _theZipFileService;
        private string thePackageManifestFileName;
		
		private string theBaseFolder;
		private string theBinFolder;

        protected override void beforeEach()
        {
			theBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "folder1");
			theBinFolder = Path.Combine(theBaseFolder, "bin");	
			
            theManifest = new PackageManifest
            {
                DataFileSet = new FileSet(),
                ContentFileSet = new FileSet()
            };

            theManifest.AddAssembly("A");
            theManifest.AddAssembly("B");
            theManifest.AddAssembly("C");

            theInput = new CreatePackageInput()
            {
                PackageFolder = theBaseFolder,
                ZipFile = Path.Combine(theBaseFolder, "package1.zip"),
                PdbFlag = false
            };

            theAssemblyFiles = new AssemblyFiles()
            {
                Files = new string[] { 
					FileSystem.Combine(theBinFolder, "a.dll"), 
					FileSystem.Combine(theBinFolder, "b.dll"), 
					FileSystem.Combine(theBinFolder, "c.dll") 
				},
                MissingAssemblies = new string[0],
                PdbFiles = new string[] { 
					FileSystem.Combine(theBinFolder, "a.pdb"), 
					FileSystem.Combine(theBinFolder, "b.pdb"), 
					FileSystem.Combine(theBinFolder, "c.pdb") 
				},
                Success = true
            };

            MockFor<IAssemblyFileFinder>()
                .Stub(x => x.FindAssemblies(theBinFolder, theManifest.Assemblies))
                .Return(theAssemblyFiles);

            _theZipFileService = new StubZipFileService();
            Services.Inject<IZipFileService>(_theZipFileService);

            thePackageManifestFileName = FileSystem.Combine(theBaseFolder, PackageManifest.FILE);

            ClassUnderTest.CreatePackage(theInput, theManifest);
        }


        [Test]
        public void should_have_written_each_pdb_to_the_zip_file()
        {
            _theZipFileService.AllEntries.ShouldNotContain(new StubZipEntry(Path.Combine(theBinFolder, "a.pdb"), "bin"));
            _theZipFileService.AllEntries.ShouldNotContain(new StubZipEntry(Path.Combine(theBinFolder, "b.pdb"), "bin"));
            _theZipFileService.AllEntries.ShouldNotContain(new StubZipEntry(Path.Combine(theBinFolder, "c.pdb"), "bin"));
        }
    }

    public class StubZipFileService : IZipFileService
    {
        private string _fileName;
        private IList<StubZipEntry> _allEntries;
        private IList<ZipFolderRequest> _requests;

        public void CreateZipFile(string fileName, Action<IZipFile> configure)
        {
            _fileName = fileName;
            var stubFile = new StubZipFile();
            configure(stubFile);

            _allEntries = stubFile.AllZipEntries;
            _requests = stubFile.ZipRequests;
        }

        public void ExtractTo(string description, Stream stream, string folder)
        {
            throw new NotImplementedException();
        }

        public void ExtractTo(string fileName, string folder, ExplodeOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetVersion(string fileName)
        {
            throw new NotImplementedException();
        }

        public IList<ZipFolderRequest> ZipRequests
        {
            get
            {
                return _requests;
            }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public IList<StubZipEntry> AllEntries
        {
            get { return _allEntries; }
        }
    }

    [TestFixture]
    public class when_trying_to_create_a_package_and_not_all_assemblies_are_found : InteractionContext<PackageCreator>
    {
        private PackageManifest theManifest;
        private AssemblyFiles theAssemblyFiles;
        private CreatePackageInput theInput;
		private string theBaseFolder;
		private string theBinFolder;
		
        protected override void beforeEach()
        {
			theBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "folder1");
			theBinFolder = Path.Combine(theBaseFolder, "bin");	
			
            theManifest = new PackageManifest{
                DataFileSet = new FileSet(),
                ContentFileSet = new FileSet()
            };

            theManifest.AddAssembly("A");
            theManifest.AddAssembly("B");
            theManifest.AddAssembly("C");

            theInput = new CreatePackageInput(){
                PackageFolder = theBaseFolder
            };

            theAssemblyFiles = new AssemblyFiles()
            {
                Files = new string[] { "a.dll", "b.dll"},
                MissingAssemblies = new string[]{"c"},
                PdbFiles = new string[] { "a.dll", "b.dll", "c.dll"},
                Success = false
            };

            MockFor<IAssemblyFileFinder>()
                .Stub(x => x.FindAssemblies(theBinFolder, theManifest.Assemblies))
                .Return(theAssemblyFiles);
        
            ClassUnderTest.CreatePackage(theInput, theManifest);
        }

        [Test]
        public void log_the_missing_assemblies()
        {
            MockFor<IPackageLogger>().AssertWasCalled(x => x.WriteAssembliesNotFound(theAssemblyFiles, theManifest, theInput));
        }

        [Test]
        public void do_not_call_the_zip_file_creator_at_all()
        {
            MockFor<IZipFileService>().AssertWasNotCalled(x => x.CreateZipFile(null, null), x => x.IgnoreArguments());
        }

    }
}