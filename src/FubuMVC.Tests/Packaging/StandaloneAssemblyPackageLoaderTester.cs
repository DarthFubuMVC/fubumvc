using System.Linq;
using System.Reflection;
using Bottles.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{
	[TestFixture]
	public class StandaloneAssemblyPackageLoaderTester : InteractionContext<StandaloneAssemblyPackageLoader>
	{
		[Test]
		public void loads_package_assemblies_from_packages_folder()
		{
			FubuMvcPackageFacility.PhysicalRootPath = "Test";
			
			MockFor<IStandaloneAssemblyFinder>()
				.Expect(finder => finder.FindAssemblies(FubuMvcPackageFacility.GetApplicationPath()))
				.Return(new[] {"TestPackage1"});

			var assembly = Assembly.Load("TestPackage1");

			ClassUnderTest
				.Load(new PackageLog())
				.Single()
				.Name
				.ShouldEqual("Assembly:  " + assembly.GetName().FullName);
		}

        [TearDown]
        public void TearDown()
        {
            FubuMvcPackageFacility.PhysicalRootPath = null;
        }
	}
}