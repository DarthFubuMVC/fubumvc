using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SharingConfigActivator : IActivator
    {
        private readonly SharingGraph _graph;
        private readonly IFileSystem _fileSystem;
        private readonly SharingRegistrationDiagnostics _diagnostics;

        public SharingConfigActivator(SharingGraph graph, IFileSystem fileSystem, SharingLogsCache logs)
        {
            _graph = graph;
            _fileSystem = fileSystem;
            _diagnostics = new SharingRegistrationDiagnostics(_graph, logs);
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            packages.Each(p => p.ForFolder(BottleFiles.WebContentFolder, folder => ReadSparkConfig(p.Name, folder, log)));
            ReadSparkConfig(FubuSparkConstants.HostOrigin, FubuMvcPackageFacility.GetApplicationPath(), log);
        }

        public void ReadSparkConfig(string provenance, string folder, IPackageLog log)
        {
            log.Trace("Looking for *spark.config in {0}", folder);
            var configs = _fileSystem.FindFiles(folder, new FileSet
            {
                Include = "*spark.config;spark.config",
                DeepSearch = false
            });

            if (!configs.Any())
            {
                log.Trace("  No *spark.config files found");
                return;
            }

            configs.Each(file => ReadFile(provenance, file, log));
        }

        public void ReadFile(string provenance, string file, IPackageLog log)
        {
            _diagnostics.SetCurrentProvenance(provenance);
            var reader = new SparkDslReader(_diagnostics);

            log.Trace("  Reading spark directives from {0}", file);
            log.TrapErrors(() => _fileSystem.ReadTextFile(file, text =>
            {
                if (text.Trim().IsEmpty())
                {
                    return;
                }

                log.TrapErrors(() => reader.ReadLine(text, provenance));
            }));
        }
    }
}