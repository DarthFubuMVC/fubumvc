using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Scanning
{
    public class SparkScannerTester
    {
        private readonly ISparkScanner _scanner;
        private readonly IEnumerable<SparkFile> _scanResult;
        private readonly TestSource _testSource;

        public SparkScannerTester()
        {
            _testSource = new TestSource();
            _scanner = new SparkScanner(new FileSystem(), new SparkFileComposer(Enumerable.Empty<ISparkFileAlteration>()));

            _scanResult = _scanner.Scan(_testSource.Paths());
        }

        [Test]
        public void all_spark_files_in_sources_are_found()
        {
            _scanResult.ShouldHaveCount(15);
        }

        [Test]
        public void correct_root_is_assigned_to_found_files()
        {
            Func<string, string> pathFor = root => _testSource.Paths().Single(p => p.Path.EndsWith(root)).Path;

            _scanResult.Where(s => s.Root == pathFor("Templates")).ShouldHaveCount(8);
            _scanResult.Where(s => s.Root == pathFor("Pak1")).ShouldHaveCount(6);
            _scanResult.Where(s => s.Root == pathFor("Pak2")).ShouldHaveCount(1);
        }        

        [Test]
        public void deepest_roots_are_searched_first()
        {
            // Design needs to be changed.
        }

        // TODO: Add more coverage
    }

    public class TestSource : IScanSource
    {
        public IEnumerable<SourcePath> Paths()
        {
            var templatePath = FileSystem.Combine(Directory.GetCurrentDirectory(), "Scanning", "Templates");             
            yield return new SourcePath{Category = SourceCategory.Host, Origin ="", Path = templatePath};
            yield return new SourcePath{Category = SourceCategory.Package, Origin ="", Path = FileSystem.Combine(templatePath, "Pak1")};
            yield return new SourcePath{Category = SourceCategory.Package, Origin ="", Path = FileSystem.Combine(templatePath, "Pak2")};
        }
    }
}