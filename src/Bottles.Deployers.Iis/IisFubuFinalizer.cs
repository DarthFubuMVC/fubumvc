using System;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Deployers.Iis
{
    // TODO -- think this should be renamed
    public class IisFubuFinalizer : IFinalizer<FubuWebsite>
    {
        private readonly IFileSystem _fileSystem;

        public IisFubuFinalizer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(FubuWebsite directive, HostManifest host, IPackageLog log)
        {
            _fileSystem.DeleteFile(FileSystem.Combine(directive.VDirPhysicalPath, "app_offline.htm"));
        }
    }
}