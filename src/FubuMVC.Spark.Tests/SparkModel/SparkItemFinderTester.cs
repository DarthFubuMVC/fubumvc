using System.IO;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core.Packaging;
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
            ClassUnderTest.HostPath = _templatePath;
        }


        [Test]
        public void finder_locates_all_relevant_spark_items()
        {
            ClassUnderTest.FindInHost().ShouldHaveCount(39);
        }

        [Test]
        public void finder_locates_all_bindings_xml()
        {
            var expected = FileSystem.Combine(_templatePath, "Shared", "bindings.xml");
            ClassUnderTest.FindInHost().ShouldContain(si => si.FilePath == expected);
        }
    }
}
