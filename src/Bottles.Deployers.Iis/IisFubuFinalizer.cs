using System;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Deployers.Iis
{
    public class IisFubuFinalizer : IFinalizer<FubuWebsite>
    {
        private IFileSystem _fileSystem;

        public IisFubuFinalizer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Finish(IDirective directive)
        {
            var d = (FubuWebsite) directive;

            _fileSystem.DeleteFile(FileSystem.Combine(d.VDirPhysicalPath, "app_offline.htm"));
        }

        public void Execute(FubuWebsite directive, HostManifest host, IPackageLog log)
        {
            throw new NotImplementedException();
        }
    }
}