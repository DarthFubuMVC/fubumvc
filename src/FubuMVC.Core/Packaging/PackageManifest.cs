using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;

namespace FubuMVC.Core.Packaging
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

        public string Name { get; set; }

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
    }
}