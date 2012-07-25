using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Runtime.Files
{
    // I vote that this hangs off BehaviorGraph during ocnfig time,
    // then gets stuffed into the container at activation time
    public interface IFubuApplicationFiles
    {
        IEnumerable<ContentFolder> AllFolders { get; }
        IEnumerable<IFubuFile> FindFiles(FileSet fileSet);

        // Who knows what you'd want to do with it, so we just return 
        // the file
        IFubuFile Find(string relativeName);
    }

    public class FubuApplicationFiles : IFubuApplicationFiles
    {
        private readonly Cache<string, IFubuFile> _files;

        private readonly Lazy<IEnumerable<ContentFolder>> _folders
            = new Lazy<IEnumerable<ContentFolder>>(ContentFolder.FindAllContentFolders);

        public FubuApplicationFiles()
        {
            _files = new Cache<string, IFubuFile>(findFile);
        }

        // I'm okay with this finding nulls

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            return _folders.Value.SelectMany(folder => folder.FindFiles(fileSet));
        }

        public IFubuFile Find(string relativeName)
        {
            return _files[relativeName.ToLower()];
        }

        public IEnumerable<ContentFolder> AllFolders
        {
            get { return _folders.Value; }
        }

        private IFubuFile findFile(string name)
        {
            var fileSet = new FileSet{
                DeepSearch = true,
                Include = name.ToLower()
            };

            return FindFiles(fileSet).FirstOrDefault();
        }
    }

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
                package.ForFolder(BottleFiles.WebContentFolder,
                                  folder => { list.Add(new ContentFolder(package.Name, folder)); });
            }

            return list;
        }

        public static ContentFolder ForApplication()
        {
            return new ContentFolder(
                Application,
                FubuMvcPackageFacility.GetApplicationPath()
                );
        }

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            return _fileSystem.FindFiles(Path, fileSet).Select(file => new FubuFile(file, Provenance));
        }
    }

    // nee FileFound from the view engine support
    public interface IFubuFile
    {
        string Path { get; }
        string Provenance { get; }
        string ReadContents();
        void ReadContents(Action<Stream> action);
        void ReadLines(Action<string> read);
    }

    public class FubuFile : IFubuFile
    {
        public FubuFile(string path, string provenance)
        {
            Path = path;
            Provenance = provenance;
        }

        public string Path { get; private set; }
        public string Provenance { get; private set; }

        public string ReadContents()
        {
            return new FileSystem().ReadStringFromFile(Path);
        }

        public void ReadContents(Action<Stream> action)
        {
            using (var stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                action(stream);
            }
        }

        public void ReadLines(Action<string> read)
        {
            new FileSystem().ReadTextFile(Path, read);
        }
    }
}