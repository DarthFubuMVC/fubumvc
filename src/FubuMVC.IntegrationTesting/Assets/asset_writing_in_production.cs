using FubuMVC.Core;
using FubuMVC.Core.Http;
using Xunit;

namespace FubuMVC.IntegrationTesting.Assets
{
    
    public class asset_writing_in_production : AssetIntegrationContext
    {
        public asset_writing_in_production()
        {
            Mode = null;
            File("Foo.js").WriteLine("var x = 0;");
        }

        [Fact]
        public void writes_the_headers_and_content()
        {
            Scenario.Get.Url("/Foo.js");

            Scenario.ContentShouldContain("var x = 0;");

            Scenario.Header(HttpGeneralHeaders.CacheControl).SingleValueShouldEqual("private, max-age=86400");
            Scenario.Header(HttpGeneralHeaders.Expires).ShouldHaveOneNonNullValue();
        }
    }
}