using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Deployers.Iis
{
    // TODO -- I think this is misnamed
    public class IisFubuInitializer : IInitializer<FubuWebsite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly DeploymentSettings _settings;

        public IisFubuInitializer(IFileSystem fileSystem, DeploymentSettings settings)
        {
            _fileSystem = fileSystem;
            _settings = settings;
        }

        public void Execute(FubuWebsite directive, HostManifest host, IPackageLog log)
        {
            if (_settings.UserForced)
            {
                log.Trace("UserForced: deleting directories");
                _fileSystem.DeleteDirectory(directive.WebsitePhysicalPath);
                _fileSystem.DeleteDirectory(directive.VDirPhysicalPath);
            }

            _fileSystem.CreateDirectory(directive.WebsitePhysicalPath);
            _fileSystem.CreateDirectory(directive.VDirPhysicalPath);

            var appOfflineFile = FileSystem.Combine(directive.VDirPhysicalPath, "app_offline.htm");

            log.Trace("Applying the application offline file");
            _fileSystem.WriteStringToFile(appOfflineFile, "&lt;html&gt;&lt;body&gt;Application is being rebuilt&lt;/body&gt;&lt;/html&gt;");
        }
    }
}