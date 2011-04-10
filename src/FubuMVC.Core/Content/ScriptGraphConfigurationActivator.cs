using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.UI.Scripts;

namespace FubuMVC.Core.Content
{
    public class ScriptGraphConfigurationActivator : IActivator
    {
        private readonly ScriptGraph _scripts;
        private readonly IFileSystem _fileSystem;

        public ScriptGraphConfigurationActivator(ScriptGraph scripts, IFileSystem fileSystem)
        {
            _scripts = scripts;
            _fileSystem = fileSystem;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            ReadScriptConfig(FubuMvcPackageFacility.GetApplicationPath(), log);
            packages.Each(p => p.ForFolder(BottleFiles.WebContentFolder, folder => ReadScriptConfig(folder, log)));

            _scripts.CompileDependencies(log);
        }

        public void ReadScriptConfig(string folder, IPackageLog log)
        {
            log.Trace("Trying to read *script.config files from {0}", folder);
            var files = _fileSystem.FindFiles(folder, new FileSet(){
                Include = "*.script.config",
                DeepSearch = false
            });

            if (!files.Any())
            {
                log.Trace("  No *.script.config files found");
                return;
            }

            files.Each(file => ReadFile(file, log));
        }

        public void ReadFile(string file, IPackageLog log)
        {
            var reader = new ScriptDslReader(_scripts);
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