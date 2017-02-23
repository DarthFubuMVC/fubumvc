using Xunit;

namespace FubuMVC.IntegrationTesting.Diagnostics
{
    
    public class can_bring_up_the_diagnostics_home_page
    {
        [Fact]
        public void get_the_200_in_normal_mode()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("_fubu");
                _.StatusCodeShouldBeOk();
            });
        }
    }
}