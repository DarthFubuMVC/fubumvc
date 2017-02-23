using AssemblyPackage;
using FubuMVC.Core;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class can_find_and_load_extensions_with_the_FubuModule_attribute
    {
        [Fact]
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