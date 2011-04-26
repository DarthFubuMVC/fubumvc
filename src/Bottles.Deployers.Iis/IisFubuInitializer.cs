using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Directives;
using FubuCore;

namespace Bottles.Deployers.Iis
{
    public class IisFubuInitializer : IInitializer<IisFubuWebsite>
    {

        private readonly IFileSystem _fileSystem;
        private readonly IDeploymentDiagnostics _diagnostics;
        private readonly DeploymentSettings _settings;

        public IisFubuInitializer(IFileSystem fileSystem, IDeploymentDiagnostics diagnostics, DeploymentSettings settings)
        {
            _fileSystem = fileSystem;
            _diagnostics = diagnostics;
            _settings = settings;
        }

        public void Initialize(IDirective directive)
        {
            _diagnostics.LogInitialization(this, directive);

            var direc = (IisFubuWebsite)directive;

            if(_settings.UserForced)
            {
                _diagnostics.LogFor(this).Trace("UserForced: deleting directories");
                _fileSystem.DeleteDirectory(direc.WebsitePhysicalPath);
                _fileSystem.DeleteDirectory(direc.VDirPhysicalPath);
            }

            _fileSystem.CreateDirectory(direc.WebsitePhysicalPath);
            _fileSystem.CreateDirectory(direc.VDirPhysicalPath);

            var appOfflineFile = FileSystem.Combine(direc.VDirPhysicalPath, "app_offline.htm");

            _fileSystem.WriteStringToFile(appOfflineFile, "&lt;html&gt;&lt;body&gt;Application is being rebuilt&lt;/body&gt;&lt;/html&gt;");
        }
    }
}