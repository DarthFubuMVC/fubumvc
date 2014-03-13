using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model.Sharing
{
    public class SharingGraphBuilder : ISharingRegistration
    {
        private readonly IFubuApplicationFiles _files;
        private readonly SharingGraph _sharingGraph = new SharingGraph();
        private IPackageLog _log;

        public SharingGraphBuilder(IFubuApplicationFiles files)
        {
            _files = files;
        }

        public SharingGraph Build()
        {
            var configs = _files.FindFiles(new FileSet
            {
                Include = "*view.config;view.config",
                DeepSearch = false
            });

            _log = PackageRegistry.Diagnostics.LogFor(_sharingGraph);
            ReadConfigs(configs, _log);

            return _sharingGraph;
        }

        public void ReadConfigs(IEnumerable<IFubuFile> configs, IPackageLog log)
        {
            SortConfigsFromApplicationLast(configs).Each(x => ReadConfig(x, log));
        }

        public void ReadConfig(IFubuFile config, IPackageLog log)
        {
            var reader = new SharingDslReader(this);

            log.Trace("  Reading sharing directives from {0}", config.ToString());
            log.TrapErrors(() => config.ReadLines(text =>
            {
                if (text.Trim().IsEmpty()) return;
                log.TrapErrors(() => reader.ReadLine(text, config.Provenance));
            }));
        }

        public static IEnumerable<IFubuFile> SortConfigsFromApplicationLast(IEnumerable<IFubuFile> files)
        {
            var rearranged = files.ToList();
            return rearranged.Where(x => x.Provenance != ContentFolder.Application)
                .Concat(rearranged.Where(x => x.Provenance == ContentFolder.Application));
        }

        public void Global(string global)
        {
            _log.Trace("Sharing '{0}' globally for views", global);
            _sharingGraph.Global(global);
        }

        public void Dependency(string dependent, string dependency)
        {
            _log.Trace("Sharing '{0}' to '{1}'", dependency, dependent);
            _sharingGraph.Dependency(dependency, dependency);
        }
    }
}