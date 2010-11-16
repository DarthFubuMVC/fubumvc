using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace FubuMVC.Core.Packaging
{
    [XmlType("package-includes")]
    public class PackageIncludeManifest
    {
        public static readonly string FILE = ".fubu-includes";
        private readonly IList<string> _folders = new List<string>();

        [XmlElement("include")]
        public string[] Folders
        {
            get
            {
                return _folders.ToArray();
            }
            set
            {
                _folders.Clear();
                if (value != null) _folders.AddRange(value.Select(x => x.ToLower()));
            }
        }

        public bool Include(string folder)
        {
            if (_folders.Contains(folder.ToLower()))
            {
                return false;
            }

            _folders.Add(folder.ToLower());
            return true;
        }

        public void Exclude(string folder)
        {
            _folders.Remove(folder.ToLower());
        }
    }
}