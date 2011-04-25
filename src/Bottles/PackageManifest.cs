using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;

namespace Bottles
{
    [XmlType("package")]
    public class PackageManifest
    {
        public static readonly string FILE = ".package-manifest";

        public PackageManifest()
        {
            DataFileSet = new FileSet();
            ContentFileSet = new FileSet(){
                Include = "*.as*x;*.master;Content{0}*.*;*.config".ToFormat(Path.DirectorySeparatorChar)
            };
        }

        private readonly IList<string> _assemblies = new List<string>();
        public static readonly string APPLICATION_MANIFEST_FILE = ".fubu-manifest";
        private readonly IList<string> _folders = new List<string>();

        public string Name { get; set; }
        public string BinPath { get; set; }

        [XmlElement("assembly")]
        public string[] Assemblies 
        {
            get
            {
                return _assemblies.ToArray();
            }
            set
            {
                _assemblies.Clear();

                if (value == null) return;
                _assemblies.AddRange(value);
            }
        }

        public bool AddAssembly(string assemblyName)
        {
            if (_assemblies.Contains(assemblyName))
            {
                return false;
            }

            _assemblies.Add(assemblyName);
            return true;
        }

        public FileSet DataFileSet
        {
            get; set;
        }

        public FileSet ContentFileSet
        {
            get; set;
        }

        [XmlElement("include")]
        public string[] LinkedFolders
        {
            get
            {
                return _folders.ToArray();
            }
            set
            {
                _folders.Clear();
                if (value != null) _folders.AddRange(value);
            }
        }

        public string EnvironmentClassName { get; set; }
        public string EnvironmentAssembly { get; set; }
        public string ConfigurationFile { get; set; }

        public bool AddLink(string folder)
        {
            if (_folders.Contains(folder))
            {
                return false;
            }

            _folders.Add(folder);
            return true;
        }

        public void RemoveLink(string folder)
        {
            _folders.Remove(folder);
        }

        public void RemoveAllLinkedFolders()
        {
            _folders.Clear();
        }

        public override string ToString()
        {
            return string.Format("Package: {0}", Name);
        }
    }
}