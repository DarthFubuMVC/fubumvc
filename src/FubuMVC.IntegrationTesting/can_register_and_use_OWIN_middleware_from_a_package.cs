using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class can_register_and_use_OWIN_middleware_from_a_package : SharedHarnessContext
    {
        [Test]
        public void should_use_a_custom_middleware_registered_from_a_bottle()
        {
            // This header should be added by the BondVillainMiddleware class
            // registered from the OwinBottle assembly
            endpoints.Get<OwinPackageEndpoint>(x => x.get_stuff_from_bottle())
                .ResponseHeaderFor("Slow-Moving")
                .ShouldEqual("Laser");
        }
    }

    public class OwinPackageEndpoint
    {
        public string get_stuff_from_bottle()
        {
            return "Check the headers";
        }
    }
}