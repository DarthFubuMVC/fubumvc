using System.Linq;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;
namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture]
    public class assembly_marked_as_FubuModule_is_added_as_a_package : FubuRegistryHarness
    {
        [Test]
        public void has_endpoints_from_AssemblyPackage()
        {
            remote.All().EndpointsForAssembly("AssemblyPackage").Any().ShouldBeTrue();
        }
    }
}