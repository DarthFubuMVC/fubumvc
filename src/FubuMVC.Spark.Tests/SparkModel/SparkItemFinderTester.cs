using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class SparkItemFinderTester : InteractionContext<SparkItemFinder>
    {
        private readonly string _templatePath;

        public SparkItemFinderTester()
        {
            _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        }

        protected override void beforeEach()
        {
            Services.Inject<IFileScanner>(new FileScanner());
        }

        [Test]
        public void finder_locates_all_relevant_spark_items()
        {
            ClassUnderTest.HostPath = _templatePath;
            ClassUnderTest.FindInHost().ShouldHaveCount(48);
        }

        [Test]
        public void exclude_directory_makes_the_finder_to_skip_completely_the_directory()
        {
            ClassUnderTest.HostPath = _templatePath;
            ClassUnderTest.ExcludeHostDirectory("App");
            ClassUnderTest.ExcludeHostDirectory("Package2", "Handlers", "Shared");
            ClassUnderTest.ExcludeHostDirectory("Pak1", "Alpha", "Bravo");
            ClassUnderTest.FindInHost().ShouldHaveCount(23);
        }

        [Test]
        public void include_file_makes_the_matching_files_to_be_included_when_finding_items()
        {
            ClassUnderTest.HostPath = _templatePath;
            ClassUnderTest.IncludeFile("file.*");
            ClassUnderTest.IncludeFile("baz.*");
            ClassUnderTest.IncludeFile("dog.zoo");
            ClassUnderTest.FindInHost().ShouldHaveCount(52);
        }

        [Test]
        public void finder_locates_all_bindings_xml()
        {
            var expected = FileSystem.Combine(_templatePath, "Shared", "bindings.xml");
            ClassUnderTest.HostPath = _templatePath;
            ClassUnderTest.FindInHost().ShouldContain(si => si.FilePath == expected);
        }

        [Test]
        public void all_the_items_found_in_host_have_host_as_origin()
        {
            ClassUnderTest.HostPath = _templatePath;
            ClassUnderTest.FindInHost()
                .All(x => x.Origin == FubuSparkConstants.HostOrigin)
                .ShouldBeTrue();
        }


        [Test]
        public void all_the_items_found_in_host_have_root_as_host_path()
        {
            ClassUnderTest.HostPath = _templatePath;
            ClassUnderTest.FindInHost()
                .All(x => x.RootPath == _templatePath)
                .ShouldBeTrue();
        }


        [Test]
        public void items_found_in_host_have_set_their_filepath()
        {
            ClassUnderTest.HostPath = _templatePath;
            var items = ClassUnderTest.FindInHost().ToList();
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "A3.spark"));
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "A4.spark"));
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "Views", "F1", "G1.spark"));
            items.ShouldContain(x => x.FilePath == Path.Combine(_templatePath, "Pak1", "Alpha", "Foxtrot", "Golf.spark"));
        }


        // TODO: TESTS FOR FindInPackages
    }
}
