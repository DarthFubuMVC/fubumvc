using FubuCore;

namespace Bottles.Deployment
{
    public class ToolRepository : IToolRepository
    {
        private IProfile _profile;
        private IFileSystem _fileSystem;

        public ToolRepository(IProfile profile, IFileSystem fileSystem)
        {
            _profile = profile;
            _fileSystem = fileSystem;
        }

        public void CopyTo(string toolName, string destination)
        {
            var source = _profile.GetPathForTool(toolName);
            _fileSystem.Copy(source, destination);
        }
    }
}