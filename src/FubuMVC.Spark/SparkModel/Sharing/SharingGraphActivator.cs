using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SharingGraphActivator : IActivator
    {
        private readonly SharingGraph _sharingGraph;
        private readonly IFileSystem _fileSystem;

        public SharingGraphActivator(SharingGraph sharingGraph, IFileSystem fileSystem)
        {
            _sharingGraph = sharingGraph;
            _fileSystem = fileSystem;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            ReadSparkConfig(FubuSparkConstants.HostOrigin, FubuMvcPackageFacility.GetApplicationPath(), log);
            packages.Each(p => p.ForFolder(BottleFiles.WebContentFolder, folder => ReadSparkConfig(p.Name, folder, log)));

            // add recorded ones here, or split out to separate activator.

            _sharingGraph.Global(FubuSparkConstants.HostOrigin);

            var provenances = packages.Select(p => p.Name).Union(new[] {FubuSparkConstants.HostOrigin}).ToArray();
            _sharingGraph.CompileDependencies(provenances);
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
            var reader = new SparkDslReader(_sharingGraph);
            log.Trace("  Reading spark directives from {0}", file);
            log.TrapErrors(() =>
            {
                _fileSystem.ReadTextFile(file, text =>
                {
                    if (text.Trim().IsEmpty()) return;

                    log.TrapErrors(() => reader.ReadLine(text, provenance));
                });
            });
        }
    }
}