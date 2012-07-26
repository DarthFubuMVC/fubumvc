using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    [Obsolete, MarkedForTermination("Unnecessary with IFubuApplicationFiles")]
    public class PackageFileActivator : IActivator
    {
        private readonly IPackageFiles _files;

        public PackageFileActivator(IPackageFiles files)
        {
            _files = files;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            packages.Each(pak =>
            {
                pak.ForFolder(BottleFiles.WebContentFolder, dir =>
                {
                    log.Trace("Adding directory {0} to PackageFileCache", dir);
                    _files.AddDirectory(dir);
                });
            });
        }
    }
}