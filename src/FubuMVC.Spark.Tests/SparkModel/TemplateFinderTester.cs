using System.IO;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Scanning;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateFinderTester : InteractionContext<TemplateFinder<Template>>
    {
        private readonly string _templatePath;

        private PackageInfo _pak1;
        private PackageInfo _pak2;

        private string _pak1Path;
        private string _pak2Path;

        public TemplateFinderTester()
        {
            _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        }

        protected override void beforeEach()
        {
            Services.Inject<IFileScanner>(new FileScanner());

            _pak1 = new PackageInfo("Pak1");
            _pak2 = new PackageInfo("Pak2");

            _pak1Path = Path.Combine("Templates", "Pak1");
            _pak2Path = Path.Combine("Templates", "Package2");

            Services.InjectArray<IPackageInfo>(new[] { _pak1, _pak2 });

            _pak1.RegisterFolder(BottleFiles.WebContentFolder, _pak1Path);
            _pak2.RegisterFolder(BottleFiles.WebContentFolder, _pak2Path);

            ClassUnderTest.HostPath = _templatePath;

            // This is now conventionally applied in SparkEngine
            new DefaultTemplateFinderConventions().Configure(ClassUnderTest);
        }

        [Test]
        public void finder_locates_all_relevant_spark_templates()
        {
            ClassUnderTest.FindInHost().ShouldHaveCount(52);
        }

        [Test]
        public void exclude_directory_makes_finder_skip_directory_completely()
        {
            ClassUnderTest.ExcludeHostDirectory("App");
            ClassUnderTest.ExcludeHostDirectory("Package2", "Handlers", "Shared");
            ClassUnderTest.ExcludeHostDirectory("Pak1", "Alpha", "Bravo");
            ClassUnderTest.FindInHost().ShouldHaveCount(23);
        }

        [Test]
        public void include_file_makes_matching_files_to_be_included_when_finding_templates()
        {
            ClassUnderTest.IncludeFile("file.*");
            ClassUnderTest.IncludeFile("baz.*");
            ClassUnderTest.IncludeFile("dog.zoo");

            var items = ClassUnderTest.FindInHost().ToList();
            items.ShouldContain(x => x.Name() == "file");
            items.ShouldContain(x => x.Name() == "baz");
            items.ShouldContain(x => x.Name() == "dog");
            items.ShouldHaveCount(56);
        }

        [Test]
        public void finder_locates_all_bindings_xml()
        {
            var expected = FileSystem.Combine(_templatePath, "Shared", "bindings.xml");
            ClassUnderTest.FindInHost().ShouldContain(si => si.FilePath == expected);
        }

        [Test]
        public void all_templates_found_in_host_have_host_as_origin()
        {
            ClassUnderTest.FindInHost()
                .All(x => x.Origin == TemplateConstants.HostOrigin)
                .ShouldBeTrue();
        }


        [Test]
        public void all_templates_found_in_host_have_root_as_host_path()
        {
            ClassUnderTest.FindInHost()
                .All(x => x.RootPath == _templatePath)
                .ShouldBeTrue();
        }


        [Test]
        public void templates_found_in_host_have_set_their_filepath()
        {
            var items = ClassUnderTest.FindInHost().ToList();
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "A3.spark"));
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "A4.spark"));
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "Views", "F1", "G1.spark"));
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "Pak1", "Alpha", "Foxtrot", "Golf.spark"));
        }

        [Test]
        public void find_in_packages_scans_the_packages_web_content_folder()
        {
            var items = ClassUnderTest.FindInPackages();
            items.ShouldHaveCount(16);
        }

        [Test]
        public void templates_found_in_packages_have_package_name_as_origin()
        {
            var items = ClassUnderTest.FindInPackages();

            items.Where(x => x.Origin == _pak1.Name).ShouldHaveCount(8);
            items.Where(x => x.Origin == _pak2.Name).ShouldHaveCount(8);
        }

        [Test]
        public void templates_found_in_packages_have_package_name_web_content_folder_as_root_path()
        {
            var items = ClassUnderTest.FindInPackages();

            items.Where(x => x.RootPath == _pak1Path).ShouldHaveCount(8);
            items.Where(x => x.RootPath == _pak2Path).ShouldHaveCount(8);
        }

        [Test]
        public void find_in_packages_takes_into_account_file_filters()
        {
            ClassUnderTest.IncludeFile("*.txt");
            ClassUnderTest.IncludeFile("sample.*");
            var items = ClassUnderTest.FindInPackages();

            items.Where(x => x.Origin == _pak1.Name).Where(x => x.Name() == "data").ShouldHaveCount(1);
            items.Where(x => x.Origin == _pak2.Name).Where(x => x.Name() == "sample").ShouldHaveCount(1);

            items.ShouldHaveCount(18);
        }
    }
}
