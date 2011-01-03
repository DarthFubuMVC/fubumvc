using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace FubuMVC.Core.Packaging
{
    [XmlType("application")]
    public class ApplicationManifest
    {
        public static readonly string FILE = ".fubu-manifest";
        private readonly IList<string> _folders = new List<string>();

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
                if (value != null) _folders.AddRange(value.Select(x => x.ToLower()));
            }
        }

        public string EnvironmentClassName { get; set; }
        public string EnvironmentAssembly { get; set; }
        public string ConfigurationFile { get; set; }

        public bool AddLink(string folder)
        {
            if (_folders.Contains(folder.ToLower()))
            {
                return false;
            }

            _folders.Add(folder.ToLower());
            return true;
        }

        public void RemoveLink(string folder)
        {
            _folders.Remove(folder.ToLower());
        }

        public void RemoveAllLinkedFolders()
        {
            _folders.Clear();
        }
    }
}