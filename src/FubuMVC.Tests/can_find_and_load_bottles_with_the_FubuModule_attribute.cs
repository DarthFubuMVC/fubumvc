using AssemblyPackage;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class can_find_and_load_extensions_with_the_FubuModule_attribute
    {
        [Test]
        public void find_assembly_bottles()
        {
            using (var runtime = FubuApplication.For(new FubuRegistry()).StructureMap().Bootstrap())
            {
                var assembly = typeof (AssemblyPackageMarker).Assembly;

                runtime.Behaviors.PackageAssemblies.ShouldContain(assembly);
            }
            ;
        }
    }
}