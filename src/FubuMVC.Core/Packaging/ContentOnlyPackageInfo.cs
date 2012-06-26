using System;
using System.IO;
using Bottles;
using Bottles.PackageLoaders.Assemblies;

namespace FubuMVC.Core.Packaging
{
    public class ContentOnlyPackageInfo : IPackageInfo
    {
        private readonly string _directory;
        private readonly string _name;

        public ContentOnlyPackageInfo(string directory, string name)
        {
            _directory = directory;
            _name = name;
            Role = BottleRoles.Module;
        }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            // Do nothing
        }

        public string Description
        {
            get { return "CONTENT ONLY"; }
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            if (folderName == BottleFiles.WebContentFolder)
            {
                onFound(_directory);
            }
        }

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            // Do nothing
        }

        public void ForFiles(string directory, string searchPattern, Action<string, Stream> fileCallback)
        {
            //do nothing
        }

        public Dependency[] Dependencies
        {
            get { return new Dependency[0]; }
        }

        public PackageManifest Manifest
        {
            get { throw new NotImplementedException(); }
        }

        public Bottles.IPackageFiles Files
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Role { get; set; }
    }
}