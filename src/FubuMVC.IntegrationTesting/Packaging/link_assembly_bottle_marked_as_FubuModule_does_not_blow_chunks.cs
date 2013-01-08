using System.Linq;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Packaging
{
    // Mono does not like the test setup on this one
    [TestFixture, Explicit]
    public class link_assembly_bottle_marked_as_FubuModule_does_not_blow_chunks : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles("link harness src/AssemblyPackage");
        }

        [Test]
        public void has_endpoints_from_AssemblyPackage()
        {
            remote.All().EndpointsForAssembly("AssemblyPackage").Any().ShouldBeTrue();
        }
    }
}