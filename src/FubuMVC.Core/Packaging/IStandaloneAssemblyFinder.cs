using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
	public interface IStandaloneAssemblyFinder
	{
		IEnumerable<string> FindAssemblies(string applicationDirectory);
	}
}