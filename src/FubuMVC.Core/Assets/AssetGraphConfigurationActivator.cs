using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Assets
{
    public class AssetGraphConfigurationActivator : IActivator
    {
        private readonly AssetGraph _assets;
        private readonly IFileSystem _fileSystem;
        private readonly AssetRegistrationDiagnostics _diagnostics;

        private static readonly IList<string> _configurationFiles = new List<string>();

        public static IEnumerable<string> ConfigurationFiles
        {
            get { return _configurationFiles; }
        }



        public AssetGraphConfigurationActivator(AssetGraph assets, IFileSystem fileSystem, AssetLogsCache logs)
        {
            _assets = assets;
            _fileSystem = fileSystem;
            _diagnostics = new AssetRegistrationDiagnostics(_assets, logs);
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _configurationFiles.Clear();

            ReadScriptConfig(FubuMvcPackageFacility.GetApplicationPath(), log);
            packages.Each(p => p.ForFolder(BottleFiles.WebContentFolder, folder => ReadScriptConfig(folder, log)));

            _assets.Precompile();

            _assets.CompileDependencies(log);
        }

        public void ReadScriptConfig(string folder, IPackageLog log)
        {
            log.Trace("Trying to read *script.config / *asset.config files from {0}", folder);
            var files = _fileSystem.FindFiles(folder, new FileSet
                                                      {
                                                          Include = "*.script.config;*.asset.config",
                                                          DeepSearch = true
                                                      });

            if (!files.Any())
            {
                log.Trace("  No *.script.config or *.asset.config files found");
                return;
            }

            files.Where(x => !x.PathRelativeTo(folder).Contains(FubuMvcPackageFacility.FubuContentFolder)).Each(file => ReadFile(file, log));
        }

        public void ReadFile(string file, IPackageLog log)
        {

            _diagnostics.SetCurrentProvenance(file);
            var reader = new AssetDslReader(_diagnostics);
            _configurationFiles.Fill(file.ToFullPath());

            log.Trace("  Reading script directives from {0}", file);
            log.TrapErrors(() =>
            {
                _fileSystem.ReadTextFile(file, text =>
                {
                    if (text.Trim().IsEmpty()) return;

                    log.TrapErrors(() => reader.ReadLine(text));
                });
            });
        }
    }
}
