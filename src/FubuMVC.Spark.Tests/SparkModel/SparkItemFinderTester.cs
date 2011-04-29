using System.Collections.Generic;
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
    public class SparkItemFinderTester
    {
        private readonly SparkItemFinder _itemFinder;
        private readonly IEnumerable<SparkItem> _items;
        private readonly string _templatePath;

        public SparkItemFinderTester()
        {
            _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _itemFinder = new SparkItemFinder(new FileScanner(), Enumerable.Empty<IPackageInfo>()) { HostPath = _templatePath };
            _itemFinder.ExcludeHostDirectory("App", FubuMvcPackageFacility.FubuPackagesFolder);
            _items = _itemFinder.FindInHost();
        }

        [Test]
        public void finder_locates_all_relevant_spark_items()
        {
            _items.ShouldHaveCount(39);
        }

        [Test]
        public void finder_locates_all_bindings_xml()
        {
            var expected = FileSystem.Combine(_templatePath, "Shared", "bindings.xml");
            _items.ShouldContain(si => si.FilePath == expected);
        }
    }
}
