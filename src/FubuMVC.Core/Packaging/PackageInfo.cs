using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Util;

namespace FubuMVC.Core.Packaging
{
    public class PackageInfo : IPackageInfo
    {
        private readonly IList<AssemblyTarget> _assemblies = new List<AssemblyTarget>();
        private readonly Cache<string, string> _directories = new Cache<string,string>();

        public PackageInfo(string name)
        {
            Name = name;
        }

        public void RegisterAssemblyLocation(string assemblyName, string filePath)
        {
            _assemblies.Add(new AssemblyTarget(){
                AssemblyName = assemblyName, 
                FilePath = filePath
            });
        }

        public class AssemblyTarget
        {
            public string AssemblyName { get; set;}
            public string FilePath { get; set;}

            public void Load(IAssemblyRegistration loader)
            {
                loader.LoadFromFile(FilePath, AssemblyName);
            }
        }

        public string Name { get; private set; }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            _assemblies.Each(a => a.Load(loader));
        }

        public void RegisterFolder(string folderName, string directory)
        {
            _directories[folderName] = directory;
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            _directories.WithValue(folderName, onFound);
        }

        //public IEnumerable<Assembly> Assemblies { get; set; }
        //public string FilesFolder { get; set; }
    }
}