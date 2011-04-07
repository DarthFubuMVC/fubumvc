using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace Bottles
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
            var dirParts = searchPattern.Split(Path.DirectorySeparatorChar);

            var folderPath = _directories[BottleFiles.DataFolder].ToFullPath();
            var filePattern = searchPattern;

            if (dirParts.Count() > 1)
            {
                var rootDir = dirParts.Take(dirParts.Length - 1).Join(Path.DirectorySeparatorChar.ToString());
                folderPath = FileSystem.Combine(folderPath, rootDir);

                if (rootDir.IsNotEmpty() && !Directory.Exists(folderPath))
                {
                    return;
                }

                filePattern = dirParts.Last();
            }

            if (!Directory.Exists(folderPath)) return;

            Directory.GetFiles(folderPath, filePattern, SearchOption.AllDirectories).Each(fileName =>
            {
                var name = fileName.PathRelativeTo(folderPath);
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {

                    dataCallback(name, stream);
                }
            });
        }
    }
}