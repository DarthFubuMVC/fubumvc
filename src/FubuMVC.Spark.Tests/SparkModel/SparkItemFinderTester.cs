using System.IO;
using FubuCore;
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

        // TODO: TESTS FOR FindInPackages
    }
}
