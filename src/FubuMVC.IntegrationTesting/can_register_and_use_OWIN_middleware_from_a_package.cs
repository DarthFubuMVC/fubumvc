using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting
{
    
    public class can_register_and_use_OWIN_middleware_from_a_package 
    {
        [Fact]
        public void should_use_a_custom_middleware_registered_from_a_bottle()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<OwinPackageEndpoint>(x => x.get_stuff_from_bottle());
                _.Header("Slow-Moving").SingleValueShouldEqual("Laser");
            });

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