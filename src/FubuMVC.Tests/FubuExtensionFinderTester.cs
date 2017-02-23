using System.Linq;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class FubuExtensionFinderTester
    {
        [Fact]
        public void do_not_import_extensions_marked_with_DoNotAutoImport()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.FindAllExtensions(new ActivationDiagnostics());

            types.OfType<FubuExtensionFinder.Importer<GoodExtension>>().Any().ShouldBeTrue();
            types.OfType<FubuExtensionFinder.Importer<NotAutoExtension>>().Any().ShouldBeFalse();

        }
    }

    public class GoodExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            throw new System.NotImplementedException();
        }
    }

    [DoNotAutoImport]
    public class NotAutoExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            throw new System.NotImplementedException();
        }
    }
}