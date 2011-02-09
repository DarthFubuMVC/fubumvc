using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageFiles
    {
        void RegisterFolder(string folderName, string directory);
        void ForFolder(string folderName, Action<string> onFound);
        void ForData(string searchPattern, Action<string, Stream> dataCallback);
    }

    public class PackageFiles : IPackageFiles
    {
        private readonly Cache<string, string> _directories = new Cache<string, string>();

        public void RegisterFolder(string folderName, string directory)
        {
            _directories[folderName] = directory;
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            _directories.WithValue(folderName, onFound);
        }

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            // Guard clause for the folder not existing
            var dirParts = searchPattern.Replace('\\', '/').Split('/');
            var dataFolderPath = _directories[FubuMvcPackages.DataFolder].ToFullPath();
            if (dirParts.Count() > 1)
            {
                var rootDir = dirParts.Take(dirParts.Length - 1).Join("/");
                if (rootDir.IsNotEmpty() && !Directory.Exists(FileSystem.Combine(dataFolderPath, rootDir))) return;
            }

            Directory.GetFiles(dataFolderPath, searchPattern, SearchOption.AllDirectories).Each(fileName =>
            {
                var name = fileName.PathRelativeTo(dataFolderPath).Replace("\\", "/");
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    dataCallback(name, stream);
                }
            });
        }
    }
}