using System.Reflection;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuExtensionFinderTester
    {
        [Test]
        public void do_not_import_extensions_marked_with_DoNotAutoImport()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = FubuExtensionFinder.FindAllExtensionTypes(new Assembly[] {assembly});

            types.ShouldContain(typeof(GoodExtension));
            types.ShouldNotContain(typeof(NotAutoExtension));
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