using System;
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
                Include = "*.as*x"
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

    public class FileSet
    {
        [XmlAttribute]
        public string Include { get; set; }

        [XmlAttribute]
        public string Exclude { get; set; }

        public IEnumerable<String> IncludedFilesFor(string path)
        {
            return getAllDistinctFiles(path, Include);
        }

        private IEnumerable<string> getAllDistinctFiles(string path, string pattern)
        {
            if (pattern.IsEmpty()) return new string[0];

            return pattern.Split(';').SelectMany(x => Directory.GetFiles(path, x)).Distinct();
        }

        public IEnumerable<string> ExcludedFilesFor(string path)
        {
            return getAllDistinctFiles(path, Exclude);
        }
    }


}