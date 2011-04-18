using FubuCore;

namespace Bottles.Deployment
{
    public class Profile : IProfile
    {
        private readonly string _basePath;

        public Profile(string basePath)
        {
            _basePath = basePath;
        }

        public string GetPathForBottle(string bottleName)
        {
            //REVIEW: hmmmm
            if (!bottleName.EndsWith("zip")) bottleName = bottleName + ".zip";

            //this should be a file
            return FileSystem.Combine(_basePath, "bottles", bottleName);
        }

        public string GetPathForTool(string toolName)
        {
            //this should be a directory
            return FileSystem.Combine(_basePath, "tools", toolName);
        }
    }
}