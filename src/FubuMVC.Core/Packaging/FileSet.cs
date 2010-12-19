using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class FileSet
    {
        public FileSet()
        {
            Include = "*.*";
        }

        [XmlAttribute]
        public string Include { get; set; }

        [XmlAttribute]
        public string Exclude { get; set; }

        public IEnumerable<string> IncludedFilesFor(string path)
        {
            return getAllDistinctFiles(path, Include.IsEmpty() ? "*.*" : Include);
        }

        private IEnumerable<string> getAllDistinctFiles(string path, string pattern)
        {
            if (pattern.IsEmpty()) return new string[0];

            return pattern.Split(';').SelectMany(x => Directory.GetFiles(path, x, SearchOption.AllDirectories)).Distinct<string>();
        }

        public IEnumerable<string> ExcludedFilesFor(string path)
        {
            return getAllDistinctFiles(path, Exclude);
        }
    }
}