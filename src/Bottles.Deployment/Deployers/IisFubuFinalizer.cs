using FubuCore;

namespace Bottles.Deployment.Deployers
{
    public class IisFubuFinalizer : IFinalizer<IisFubuWebsite>
    {
        private IFileSystem _fileSystem;

        public IisFubuFinalizer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Finish(IDirective directive)
        {
            var d = (IisFubuWebsite) directive;

            _fileSystem.DeleteFile(FileSystem.Combine(d.VDirPhysicalPath, "app_offline.htm"));
        }
    }
}