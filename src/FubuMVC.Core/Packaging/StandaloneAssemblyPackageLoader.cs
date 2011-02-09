using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
	public class StandaloneAssemblyPackageLoader : IPackageLoader
	{
		private readonly IStandaloneAssemblyFinder _assemblyFinder;

		public StandaloneAssemblyPackageLoader(IStandaloneAssemblyFinder assemblyFinder)
		{
			_assemblyFinder = assemblyFinder;
		}

		public IEnumerable<IPackageInfo> Load()
		{
			var assemblies = _assemblyFinder.FindAssemblies(FubuMvcPackageFacility.GetApplicationPath());
            return assemblies.Select(assembly => AssemblyPackageInfo.CreateFor(Assembly.Load(assembly)).As<IPackageInfo>());
		}
	}
}