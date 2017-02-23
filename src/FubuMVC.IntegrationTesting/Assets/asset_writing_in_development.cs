using FubuMVC.Core;
using FubuMVC.Core.Http;
using Xunit;

namespace FubuMVC.IntegrationTesting.Assets
{
    
    public class asset_writing_in_development : AssetIntegrationContext
    {
        public asset_writing_in_development()
        {
            Mode = "development";
            File("Foo.js").WriteLine("var x = 0;");
        }

        [Fact]
        public void doesn_not_writes_the_headers_and_content()
        {
            Scenario.Get.Url("/Foo.js");

            Scenario.ContentShouldContain("var x = 0;");

            Scenario.Header(HttpGeneralHeaders.CacheControl).ShouldNotBeWritten();
            Scenario.Header(HttpGeneralHeaders.Expires).ShouldNotBeWritten();
        }
    }
}