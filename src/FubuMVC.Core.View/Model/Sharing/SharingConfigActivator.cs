using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model.Sharing
{
    public class SharingConfigActivator : IActivator
    {
        private readonly SharingGraph _sharingGraph;
        private readonly IFubuApplicationFiles _files;
        private readonly SharingRegistrationDiagnostics _diagnostics; 

        public SharingConfigActivator(SharingGraph sharingGraph, SharingLogsCache logs, IFubuApplicationFiles files)
        {
            _sharingGraph = sharingGraph;
            _diagnostics = new SharingRegistrationDiagnostics(_sharingGraph, logs);
            _files = files;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            log.Trace("Looking for *view.config files");
            var configs = _files.FindFiles(new FileSet
            {
                Include = "*view.config;view.config",
                DeepSearch = false
            });

            ReadConfigs(configs, log);
        }

        public void ReadConfigs(IEnumerable<IFubuFile> configs, IPackageLog log)
        {
            SortConfigsFromApplicationLast(configs).Each(x => ReadConfig(x, log));            
        }

        public void ReadConfig(IFubuFile config, IPackageLog log)
        {
            _diagnostics.SetCurrentProvenance(config.Provenance);
            var reader = new SharingDslReader(_diagnostics);

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
    }
}