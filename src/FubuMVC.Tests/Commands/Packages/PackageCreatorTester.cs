using System;
using System.Collections.Generic;
using System.IO;
using Fubu.Packages;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
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

        protected override void beforeEach()
        {
            theManifest = new PackageManifest
            {
                Assemblies = "A;B;C",
                DataFileSet = new FileSet(),
                ContentFileSet = new FileSet()
            };

            theInput = new CreatePackageInput()
            {
                PackageFolder = "c:\\folder1",
                ZipFile = "c:\\package1.zip",
                PdbFlag = true
            };

            theAssemblyFiles = new AssemblyFiles()
            {
                Files = new string[] { "c:\\folder1\\bin\\a.dll", "c:\\folder1\\bin\\b.dll", "c:\\folder1\\bin\\c.dll" },
                MissingAssemblies = new string[0],
                PdbFiles = new string[] { "c:\\folder1\\bin\\a.pdb", "c:\\folder1\\bin\\b.pdb", "c:\\folder1\\bin\\c.pdb" },
                Success = true
            };

            MockFor<IAssemblyFileFinder>()
                .Stub(x => x.FindAssemblies("c:\\folder1\\bin", theManifest.AssemblyNames))
                .Return(theAssemblyFiles);

            _theZipFileService = new StubZipFileService();
            Services.Inject<IZipFileService>(_theZipFileService);

            thePackageManifestFileName = FileSystem.Combine("c:\\folder1", PackageManifest.FILE);

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
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry("c:\\folder1\\bin\\a.dll", "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry("c:\\folder1\\bin\\b.dll", "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry("c:\\folder1\\bin\\c.dll", "bin"));
        }

        [Test]
        public void should_have_written_each_pdb_to_the_zip_file()
        {
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry("c:\\folder1\\bin\\a.pdb", "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry("c:\\folder1\\bin\\b.pdb", "bin"));
            _theZipFileService.AllEntries.ShouldContain(new StubZipEntry("c:\\folder1\\bin\\c.pdb", "bin"));
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
                ZipDirectory = FubuMvcPackages.WebContentFolder,
                RootDirectory = theInput.PackageFolder
            });
        }

        [Test]
        public void add_the_data_files()
        {
            _theZipFileService.ZipRequests.ShouldContain(new ZipFolderRequest()
            {
                FileSet = theManifest.DataFileSet,
                ZipDirectory = PackageInfo.DataFolder,
                RootDirectory = Path.Combine(theInput.PackageFolder, PackageInfo.DataFolder)
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

        protected override void beforeEach()
        {
            theManifest = new PackageManifest
            {
                Assemblies = "A;B;C",
                DataFileSet = new FileSet(),
                ContentFileSet = new FileSet()
            };

            theInput = new CreatePackageInput()
            {
                PackageFolder = "c:\\folder1",
                ZipFile = "c:\\package1.zip",
                PdbFlag = false
            };

            theAssemblyFiles = new AssemblyFiles()
            {
                Files = new string[] { "c:\\folder1\\bin\\a.dll", "c:\\folder1\\bin\\b.dll", "c:\\folder1\\bin\\c.dll" },
                MissingAssemblies = new string[0],
                PdbFiles = new string[] { "c:\\folder1\\bin\\a.pdb", "c:\\folder1\\bin\\b.pdb", "c:\\folder1\\bin\\c.pdb" },
                Success = true
            };

            MockFor<IAssemblyFileFinder>()
                .Stub(x => x.FindAssemblies("c:\\folder1\\bin", theManifest.AssemblyNames))
                .Return(theAssemblyFiles);

            _theZipFileService = new StubZipFileService();
            Services.Inject<IZipFileService>(_theZipFileService);

            thePackageManifestFileName = FileSystem.Combine("c:\\folder1", PackageManifest.FILE);

            ClassUnderTest.CreatePackage(theInput, theManifest);
        }


        [Test]
        public void should_have_written_each_pdb_to_the_zip_file()
        {
            _theZipFileService.AllEntries.ShouldNotContain(new StubZipEntry("c:\\folder1\\bin\\a.pdb", "bin"));
            _theZipFileService.AllEntries.ShouldNotContain(new StubZipEntry("c:\\folder1\\bin\\b.pdb", "bin"));
            _theZipFileService.AllEntries.ShouldNotContain(new StubZipEntry("c:\\folder1\\bin\\c.pdb", "bin"));
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

        public void ExtractTo(string fileName, string folder)
        {
            throw new NotImplementedException();
        }

        public Guid GetVersion(string fileName)
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

        protected override void beforeEach()
        {
            theManifest = new PackageManifest{
                Assemblies = "A;B;C",
                DataFileSet = new FileSet(),
                ContentFileSet = new FileSet()
            };

            theInput = new CreatePackageInput(){
                PackageFolder = "c:\\folder1"
            };

            theAssemblyFiles = new AssemblyFiles()
            {
                Files = new string[] { "a.dll", "b.dll"},
                MissingAssemblies = new string[]{"c"},
                PdbFiles = new string[] { "a.dll", "b.dll", "c.dll"},
                Success = false
            };

            MockFor<IAssemblyFileFinder>()
                .Stub(x => x.FindAssemblies("c:\\folder1\\bin", theManifest.AssemblyNames))
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