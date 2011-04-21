using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Bottles;

namespace FubuMVC.Core.Packaging
{
	[XmlType("application")]
	public class ApplicationManifest : PackageManifest
	{
		public static readonly string APPLICATION_MANIFEST_FILE = ".fubu-manifest";
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
	}
}