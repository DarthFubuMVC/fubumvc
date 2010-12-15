using System;
using System.Collections.Generic;
using System.IO;
using FubuCore.Util;

namespace FubuMVC.Core.Packaging
{
    public class PackageInfo : IPackageInfo
    {
        public static readonly string DataFolder = "Data";

        private readonly IList<AssemblyTarget> _assemblies = new List<AssemblyTarget>();
        private readonly Cache<string, string> _directories = new Cache<string,string>();

        public PackageInfo(string name)
        {
            Name = name;
        }

        public string Description
        {
            get; set;
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

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            Directory.GetFiles(_directories[DataFolder], searchPattern, SearchOption.AllDirectories).Each(fileName =>
            {
                var name = Path.GetFileName(fileName);
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    dataCallback(name, stream);
                }
            });
        }

        //public IEnumerable<Assembly> Assemblies { get; set; }
        //public string FilesFolder { get; set; }

        public override string ToString()
        {
            return Description;
        }

        public bool Equals(PackageInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Description, Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PackageInfo)) return false;
            return Equals((PackageInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Description != null ? Description.GetHashCode() : 0);
            }
        }
    }
}