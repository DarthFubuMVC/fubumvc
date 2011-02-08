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
			return _fileSystem.FindFiles(FileSystem.Combine(applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder), assemblyNames).Select(Path.GetFileNameWithoutExtension);
		}
	}
}