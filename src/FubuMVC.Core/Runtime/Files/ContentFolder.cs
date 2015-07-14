using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;

namespace FubuMVC.Core.Runtime.Files
{
    public class ContentFolder
    {
        public static readonly string Application = "Application";

        // Flyweight
        // It'll be 100% integration tests, so I don't care about DI here
        private static readonly IFileSystem _fileSystem = new FileSystem();

        public ContentFolder(string provenance, string path)
        {
            Provenance = provenance;
            Path = path;
        }

        public string Provenance { get; private set; }
        public string Path { get; private set; }

        public static IEnumerable<ContentFolder> FindAllContentFolders()
        {
            var list = new List<ContentFolder>{
                ForApplication()
            };

            foreach (var package in PackageRegistry.Packages)
            {
                var name = package.Name;
                package.ForFolder(BottleFiles.WebContentFolder, 
                    folder => list.Add(new ContentFolder(name, folder)));
            }

            return list;
        }

        public static ContentFolder ForApplication()
        {
            return new ContentFolder(
                Application,
                FubuApplication.GetApplicationPath()
                );
        }

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            return _fileSystem.FindFiles(Path, fileSet).Select(file =>
            {
                var fubuFile = new FubuFile(file, Provenance)
                {
                    ProvenancePath = Path,
                    RelativePath = file.PathRelativeTo(Path).Replace("\\", "/")
                };

                return fubuFile;
            });
        }
    }
}