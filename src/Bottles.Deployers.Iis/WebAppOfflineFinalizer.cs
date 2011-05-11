using System;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Deployers.Iis
{
    public class WebAppOfflineFinalizer : IFinalizer<Website>
    {
        private readonly IFileSystem _fileSystem;

        public WebAppOfflineFinalizer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(Website directive, HostManifest host, IPackageLog log)
        {
            _fileSystem.DeleteFile(FileSystem.Combine(directive.VDirPhysicalPath, "app_offline.htm"));
        }
    }
}