using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Scanning
{
    [TestFixture]
    public class SparkScannerTester : InteractionContext<FileScanner>
    {
        private IList<SparkItem> _scanResult;

        protected override void beforeEach()
        {
            Services.Inject<IFileSystem>(new FileSystem());
            _scanResult = new List<SparkItem>();
            var request = new ScanRequest();
            TestSource.Paths().Each(request.AddRoot);
            request.Include("*.spark");
            request.AddHandler(file => _scanResult.Add(new SparkItem(file.Path, file.Root, "")));

            ClassUnderTest.Scan(request);
        }

        [Test]
        public void all_spark_files_in_sources_are_found()
        {
            _scanResult.ShouldHaveCount(47);
        }

        [Test]
        public void correct_root_is_assigned_to_found_files()
        {
            Func<string, string> pathFor = root => TestSource.Paths().Single(p => p.EndsWith(root));

            _scanResult.Where(s => s.RootPath == pathFor("Templates")).ShouldHaveCount(35);
            _scanResult.Where(s => s.RootPath == pathFor("Pak1")).ShouldHaveCount(8);
            _scanResult.Where(s => s.RootPath == pathFor("Pak2")).ShouldHaveCount(4);
        }

        // TODO: Add more coverage
    }

    public static class TestSource 
    {
        public static IEnumerable<string> Paths()
        {
            var templatePath = FileSystem.Combine(Directory.GetCurrentDirectory(), "Templates");
            yield return FileSystem.Combine(templatePath, "Pak1");
            yield return FileSystem.Combine(templatePath, "Pak2");
            yield return templatePath;
        }
    }
}