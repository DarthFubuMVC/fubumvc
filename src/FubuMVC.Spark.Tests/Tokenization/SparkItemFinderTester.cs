using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Tokenization
{
    [TestFixture]
    public class SparkItemFinderTester
    {
        private readonly ISparkItemFinder _itemFinder;
        private readonly IEnumerable<SparkItem> _items;
        private readonly string _templatePath;

        public SparkItemFinderTester()
        {
            _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _itemFinder = new SparkItemFinder(new FileScanner(), new[] { new SparkRoot { Path = _templatePath }});
            _items = _itemFinder.FindItems();
        }

        [Test]
        public void finder_locates_all_relevant_spark_items()
        {
            _items.ShouldHaveCount(48);
        }

        [Test]
        public void finder_locates_all_bindings_xml()
        {
            var expected = FileSystem.Combine(_templatePath, "Shared", "bindings.xml");
            _items.ShouldContain(si => si.FilePath == expected);
        }
    }
}
