using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace Bottles
{
    public interface IPackageFiles
    {
        /// <summary>
        ///   This a way to abstract the file system. You can add the directory
        /// </summary>
        /// <param name = "folderName">the name of the folder as perceived in the package</param>
        /// <param name = "directory">the actual name of the directory</param>
        void RegisterFolder(string folderName, string directory);

        /// <summary>
        ///   If you register the directory 'Content' as 'WebContent' the 'ForFolder' will
        ///   call your call back with 'Content' if you ask for folder 'WebContent'
        /// </summary>
        /// <param name = "folderName"></param>
        /// <param name = "onFound"></param>
        void ForFolder(string folderName, Action<string> onFound);

        /// <summary>
        ///   Will call the 'onFileCallback' for each file that is found matching the pattern.
        ///   If you were to type '*.jpg' you would get all of the *.jpg in the whole package
        ///   If you were to type 'images/*.jpg' you would get only the .jpg in the 'images'
        ///   directory.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="searchPattern"></param>
        /// <param name="onFileCallback"></param>
        /// <returns></returns>
        bool GetFiles(string folderName, string searchPattern, Action<string, Stream> onFileCallback);
    }

    [DebuggerDisplay("{debuggerDisplay()}")]
    public class PackageFiles : IPackageFiles
    {
        private readonly Cache<string, string> _directories = new Cache<string, string>();

        
        public void RegisterFolder(string folderName, string directory)
        {
            if (folderName.Contains(Path.PathSeparator))
            {
                throw new ArgumentException(
                    "The path you have provided '{0}' contains a PathSeparator ('{1}') please do not register anything but root directories."
                        .ToFormat(folderName, Path.PathSeparator));
            }

            _directories[folderName] = directory;
        }


        public void ForFolder(string folderName, Action<string> onFound)
        {
            _directories.WithValue(folderName, onFound);
        }

        
        public bool GetFiles(string folderName, string searchPattern, Action<string, Stream> onFileCallback)
        {
            if (!_directories.Has(folderName))
            {
                return false;
            }

            var dirParts = searchPattern.Split(Path.DirectorySeparatorChar);
            var folderPath = _directories[folderName].ToFullPath();
            var filePattern = searchPattern;

            if (dirParts.Count() > 1)
            {
                var rootDir = dirParts.Take(dirParts.Length - 1).Join(Path.DirectorySeparatorChar.ToString());
                folderPath = folderPath.AppendPath(rootDir);

                if (rootDir.IsNotEmpty() && !Directory.Exists(folderPath))
                {
                    return false;
                }

                filePattern = dirParts.Last();
            }

            if (!Directory.Exists(folderPath)) return false; //false because the path didn't even exist

            // Use foreach as on mono you will get an internal compile error when using the standard
            // string[].Each( fileName => { onFileCallback(....) }) pattern.
            // https://bugzilla.xamarin.com/show_bug.cgi?id=16513
            foreach(var fileName in Directory.GetFiles(folderPath, filePattern, SearchOption.AllDirectories))
            {
                var name = fileName.PathRelativeTo(folderPath);
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    onFileCallback(name, stream);
                }
            }
            return true;

        }

        private string debuggerDisplay()
        {
            return "Directories: {0}".ToFormat(_directories.Count);
        }
    }
}