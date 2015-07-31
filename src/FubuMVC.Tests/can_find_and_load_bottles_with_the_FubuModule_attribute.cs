using AssemblyPackage;
using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class can_find_and_load_extensions_with_the_FubuModule_attribute
    {
        [Test]
        public void find_assembly_extensions()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var assembly = typeof (AssemblyPackageMarker).Assembly;

                runtime.Behaviors.PackageAssemblies.ShouldContain(assembly);
            }
            ;
        }
    }
}