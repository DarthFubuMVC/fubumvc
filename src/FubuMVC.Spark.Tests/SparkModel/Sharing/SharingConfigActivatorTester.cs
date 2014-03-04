using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model.Sharing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    // TODO: Simplify this, no need to touch the FS.

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
            Directory.GetFiles(".", "*view.config").Each(File.Delete);

            _graph = new SharingGraph();
            _fileSystem = new FileSystem();
            _packageLog = new PackageLog();
            _sharingLogs = new SharingLogsCache();

            _activator = new SharingConfigActivator(_graph, _sharingLogs, new FubuApplicationFiles());
        }

        [Test]
        public void read_a_directory()
        {
            var imports = new StringBuilder();
            imports.AppendLine("import from Pak2.Design, Pak2.Bindings");
            imports.AppendLine("import from Pak1");

            var exports = new StringBuilder();
            exports.AppendLine("export to all");

            _fileSystem.WriteStringToFile("imports.view.config", imports.ToString());
            _fileSystem.WriteStringToFile("exports.view.config", exports.ToString());

            _activator.ReadConfig(new FubuFile("imports.view.config", "Pak2.Core"), _packageLog);
            _activator.ReadConfig(new FubuFile("exports.view.config", "Pak2.Core"), _packageLog);
            
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

            _fileSystem.WriteStringToFile("view.config", config.ToString());
            _activator.ReadConfig(new FubuFile("view.config", "Prov"), _packageLog);

            _graph.CompileDependencies("Prov", "X", "Y", "M");

            _graph.SharingsFor("Prov").ShouldHaveTheSameElementsAs("X");
            _graph.SharingsFor("X").ShouldHaveCount(0);
            _graph.SharingsFor("Z").ShouldHaveTheSameElementsAs("Prov");
            _graph.SharingsFor("M").ShouldHaveCount(0);
        }

        [Test]
        public void sort_application_files_last()
        {
            var configs = new[]
            {
                new FubuFile("a.txt", ContentFolder.Application),
                new FubuFile("b.txt", "Pak1"),
                new FubuFile("c.txt", "Pak2")
            };

            SharingConfigActivator.SortConfigsFromApplicationLast(configs.ToList())
                .ShouldHaveTheSameElementsAs(configs[1], configs[2], configs[0]);
        }
    }
}
