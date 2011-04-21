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
            if (!bottleName.EndsWith(BottleFiles.Extension))
                bottleName = bottleName + "." + BottleFiles.Extension;

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