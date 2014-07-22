using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class map_files_are_whitelisted
    {
        [Test]
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