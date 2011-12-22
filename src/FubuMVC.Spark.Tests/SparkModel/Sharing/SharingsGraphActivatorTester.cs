using System.Collections.Generic;
using System.IO;
using System.Text;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class SharingConfigActivatorTester
    {
        private SharingGraph _graph;
        private SharingConfigActivator _activator;
        private IPackageLog _packageLog;
        private IFileSystem _fileSystem;
        private SharingLogsCache _sharingLogs;

        [SetUp]
        public void beforeEach()
        {
            Directory.GetFiles(".", "*spark.config").Each(File.Delete);

            _graph = new SharingGraph();
            _fileSystem = new FileSystem();
            _packageLog = new PackageLog();
            _sharingLogs = new SharingLogsCache();

            _activator = new SharingConfigActivator(_graph, _fileSystem, _sharingLogs);
        }


        [Test]
        public void read_a_directory()
        {
            var imports = new StringBuilder();
            imports.AppendLine("import from Pak2.Design, Pak2.Bindings");
            imports.AppendLine("import from Pak1");

            var exports = new StringBuilder();
            exports.AppendLine("export to all");

            _fileSystem.WriteStringToFile("imports.spark.config", imports.ToString());
            _fileSystem.WriteStringToFile("exports.spark.config", exports.ToString());

            _activator.ReadSparkConfig("Pak2.Core", ".", _packageLog);
            
            _graph.CompileDependencies("Pak1", "Pak2.Core", "Pak2.Design", "Pak2.Bindings");

            _graph.SharingsFor("Pak1").ShouldHaveTheSameElementsAs("Pak2.Core");
            _graph.SharingsFor("Pak2.Core").ShouldHaveTheSameElementsAs("Pak2.Design", "Pak2.Bindings", "Pak1");
            _graph.SharingsFor("Pak2.Design").ShouldHaveTheSameElementsAs("Pak2.Core");
            _graph.SharingsFor("Pak2.Bindings").ShouldHaveTheSameElementsAs("Pak2.Core");
        }

        [Test]
        public void read_a_file()
        {
            var config = new StringBuilder();
            config.AppendLine("import from X");
            config.AppendLine("export to Z");

            _fileSystem.WriteStringToFile("spark.config", config.ToString());
            _activator.ReadFile("Prov", "spark.config", _packageLog);

            _graph.CompileDependencies("Prov", "X", "Y", "M");

            _graph.SharingsFor("Prov").ShouldHaveTheSameElementsAs("X");
            _graph.SharingsFor("X").ShouldHaveCount(0);
            _graph.SharingsFor("Z").ShouldHaveTheSameElementsAs("Prov");
            _graph.SharingsFor("M").ShouldHaveCount(0);
        }
    }
}
