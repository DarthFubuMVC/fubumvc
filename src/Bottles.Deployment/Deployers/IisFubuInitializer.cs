using Bottles.Deployment.Diagnostics;
using FubuCore;

namespace Bottles.Deployment.Deployers
{
    public class IisFubuInitializer : IInitializer<IisFubuWebsite>
    {

        private readonly IFileSystem _fileSystem;
        private readonly IDeploymentDiagnostics _diagnostics;

        public IisFubuInitializer(IFileSystem fileSystem, IDeploymentDiagnostics diagnostics)
        {
            _fileSystem = fileSystem;
            _diagnostics = diagnostics;
        }

        public void Initialize(IDirective directive)
        {
            _diagnostics.LogInitialization(this, directive);

            var direc = (IisFubuWebsite)directive;

            _fileSystem.CreateDirectory(direc.WebsitePhysicalPath);
            _fileSystem.CreateDirectory(direc.VDirPhysicalPath);

            var appOfflineFile = FileSystem.Combine(direc.VDirPhysicalPath, "app_offline.htm");

            _fileSystem.WriteStringToFile(appOfflineFile, "&lt;html&gt;&lt;body&gt;Application is being rebuilt&lt;/body&gt;&lt;/html&gt;");
        }
    }
}