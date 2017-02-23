using Xunit;

namespace FubuMVC.IntegrationTesting.Diagnostics
{
    
    public class serving_up_embedded_diagnostic_assets
    {
        [Fact]
        public void can_deliver_up_embedded_diagnostic_asset()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("_fubu/asset/version/bootstrap.min.css");
                _.StatusCodeShouldBeOk();

                _.ContentTypeShouldBe("text/css");
                _.ContentShouldContain("Licensed under MIT");
            });
        }
    }
}