using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FubuCore
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

        public static FileSet ForAssemblyNames(IEnumerable<string> assemblyNames)
        {
            return new FileSet(){
                Exclude = null,
                Include = assemblyNames.OrderBy(x => x).Select(x => "{0}.dll;{0}.exe".ToFormat(x)).Join(";")
            };
        }

        public static FileSet ForAssemblyDebugFiles(IEnumerable<string> assemblyNames)
        {
            return new FileSet(){
                Exclude = null,
                Include = assemblyNames.OrderBy(x => x).Select(x => "{0}.pdb".ToFormat(x)).Join(";")
            };
        }

        public bool Equals(FileSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Include, Include) && Equals(other.Exclude, Exclude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FileSet)) return false;
            return Equals((FileSet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Include != null ? Include.GetHashCode() : 0)*397) ^ (Exclude != null ? Exclude.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Include: {0}, Exclude: {1}", Include, Exclude);
        }
    }
}