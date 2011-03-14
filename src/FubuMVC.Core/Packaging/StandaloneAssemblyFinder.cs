using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
	public class StandaloneAssemblyFinder : IStandaloneAssemblyFinder
	{
		private readonly IFileSystem _fileSystem;

		public StandaloneAssemblyFinder(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public IEnumerable<string> FindAssemblies(string applicationDirectory)
		{
			var assemblyNames = new FileSet { Include = "*.dll", DeepSearch = false };
		    var directory = PackageExploder.GetPackageDirectory(applicationDirectory);
		    return _fileSystem.FindFiles(directory, assemblyNames).Select(Path.GetFileNameWithoutExtension);
		}
	}
}