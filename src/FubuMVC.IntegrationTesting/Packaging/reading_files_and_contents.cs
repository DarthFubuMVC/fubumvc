using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using System.Collections.Generic;
using StringWriter = System.IO.StringWriter;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Packaging
{
    // Too undependable for the CI box
    [TestFixture, Explicit]
    public class reading_files_and_contents : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"link harness --clean-all");

            runBottles(@"
init src/TestPackage1 pak1
link harness pak1

init src/TestPackage2 pak2
link harness pak2
");
        }

        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<FileReaderEndpoint>();
        }

        [Test]
        public void all_of_the_folders_exist()
        {
            var folders = endpoints.Get<FileReaderEndpoint>(x => x.get_folders())
                .ReadAsText().ReadLines();

            folders.Select(x => x.Split('|').First())
                .ShouldHaveTheSameElementsAs("Application", "AssemblyPackage", "pak1", "pak2");
        }


        [Test]
        public void can_find_all_the_text_files_from_the_packages_and_application()
        {
            var files = endpoints.Get<FileReaderEndpoint>(x => x.get_txt_files())
                .ReadAsText().ReadLines();


            files.Any(x => x.Contains(FubuMvcPackageFacility.FubuContentFolder)).ShouldBeFalse();

            files.ShouldContain("bin/Debug/Runtime/Files/Data/a.txt");
            files.ShouldContain("bin/Debug/Runtime/Files/Data/b.txt");
            files.ShouldContain("bin/Debug/Runtime/Files/Data/c.txt");
            files.ShouldContain("bin/Debug/Runtime/Files/Data/d.txt");

            // one of the packages
            files.ShouldContain("data/a.txt");
        }

        [Test]
        public void can_read_a_file()
        {
            endpoints.Get<FileReaderEndpoint>(x => x.get_read_file())
                .ReadAsText().ShouldEqual("content from 1.txt");
        }
    }


    public class FileReaderEndpoint
    {
        private readonly IFubuApplicationFiles _files;
        private readonly StringWriter _writer = new StringWriter();

        public FileReaderEndpoint(IFubuApplicationFiles files)
        {
            _files = files;
        }

        public string get_folders()
        {
            _files.AllFolders.Each(folder => _writer.WriteLine(folder.Provenance +"|" + folder.Path));

            return _writer.ToString();
        }

        public string get_txt_files()
        {
            _files.FindFiles(new FileSet{
                DeepSearch = true,
                Include = "*.txt"
            }).Each(x => _writer.WriteLine(x.RelativePath));

            return _writer.ToString();
        }

        public string get_read_file()
        {
            var relativePath = "Files/1.txt";


            var file = _files.Find(relativePath);

            if (file == null)
            {
                throw new InvalidOleVariantTypeException();
            }

            return file.ReadContents();
        }

    }

    public class FilePath : ResourcePath
    {
        public FilePath(string path) : base(path)
        {
        }
    }
}