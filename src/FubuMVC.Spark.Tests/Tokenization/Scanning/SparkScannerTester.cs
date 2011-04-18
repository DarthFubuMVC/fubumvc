using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Tokenization.Scanning
{
    public class SparkScannerTester
    {
        private readonly IFileScanner _scanner;
        private readonly IList<SparkItem> _scanResult;

        public SparkScannerTester()
        {
            _scanner = new FileScanner(new FileSystem());
            _scanResult=new List<SparkItem>();
            var request = new ScanRequest();
            TestSource.Paths().Each(request.AddRoot);
            request.AddFileFilter("*.spark");
            request.AddHandler(file => _scanResult.Add(new SparkItem(file.Path, file.Root, "")));
            _scanner.Scan(request);
        }

        [Test]
        public void all_spark_files_in_sources_are_found()
        {
            _scanResult.ShouldHaveCount(15);
        }

        [Test]
        public void correct_root_is_assigned_to_found_files()
        {
            Func<string, string> pathFor = root => TestSource.Paths().Single(p => p.EndsWith(root));

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

    public static class TestSource 
    {
        public static IEnumerable<string> Paths()
        {
            var templatePath = FileSystem.Combine(Directory.GetCurrentDirectory(), "Tokenization", "Scanning", "Templates");
            yield return templatePath;
            yield return FileSystem.Combine(templatePath, "Pak1");
            yield return FileSystem.Combine(templatePath, "Pak2");
        }
    }
}