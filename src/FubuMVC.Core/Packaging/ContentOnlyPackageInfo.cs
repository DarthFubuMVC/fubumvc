using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bottles;
using Bottles.Exploding;
using Bottles.PackageLoaders.Assemblies;
using FubuCore;

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

        public static IEnumerable<ContentOnlyPackageInfo> FromAssemblies(string applicationDirectory)
        {
            var exploder = BottleExploder.GetPackageExploder(new FileSystem());
            var assemblies = FubuModuleAttributePackageLoader.FindAssemblies(new string[] {applicationDirectory.AppendPath("bin")});

            var list = new List<ContentOnlyPackageInfo>();

            foreach (var assembly in assemblies)
            {
                var package = new PackageInfo(new PackageManifest());
                exploder.ExplodeAssembly(applicationDirectory, assembly, package);
            
                package.ForFolder(BottleFiles.WebContentFolder, dir => {
                    list.Add(new ContentOnlyPackageInfo(dir, assembly.GetName().Name));
                });
            }

            return list;
        }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            // Do nothing
        }

        public string Description
        {
            get { return "CONTENT ONLY"; }
        }

        public override string ToString()
        {
            return "ContentOnlyPackage:  " + Name;
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