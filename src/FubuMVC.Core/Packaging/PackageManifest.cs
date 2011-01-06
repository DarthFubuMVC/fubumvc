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
                Include = "*.as*x;*.master;Content{0}*.*".ToFormat(Path.DirectorySeparatorChar)
            };
        }

        private readonly IList<string> _assemblyNames = new List<string>();

        public string Name { get; set; }

        public string Assemblies 
        {
            get
            {
                return _assemblyNames.Join(";");
            }
            set
            {
                var names = value.Split(';').Select(x => x.Trim());
                _assemblyNames.Clear();
                _assemblyNames.AddRange(names);
            }
        }

        public IEnumerable<string> AssemblyNames
        {
            get
            {
                return _assemblyNames;
            }
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