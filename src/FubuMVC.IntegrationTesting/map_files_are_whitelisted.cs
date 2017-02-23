using Xunit;

namespace FubuMVC.IntegrationTesting
{
    
    public class map_files_are_whitelisted
    {
        [Fact]
        public void read_MAP_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("test.map");
                _.StatusCodeShouldBeOk();
                _.ContentShouldContain("Some content in a .map file");
            });
        }
    }
}