using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace FubuMVC.Core.Packaging
{
    [XmlType("package")]
    public class PackageManifest
    {
        public static readonly string FILE = ".package-manifest";

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
    }
}