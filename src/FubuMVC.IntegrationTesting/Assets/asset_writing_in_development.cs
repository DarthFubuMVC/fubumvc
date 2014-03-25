using FubuMVC.Core;
using FubuMVC.Core.Http;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class asset_writing_in_development : AssetIntegrationContext
    {
        public asset_writing_in_development()
        {
            FubuMode.SetUpForDevelopmentMode();
            File("Foo.js").WriteLine("var x = 0;");
        }

        [Test]
        public void doesn_not_writes_the_headers_and_content()
        {
            Scenario.Get.Url("/Foo.js");

            Scenario.ContentShouldContain("var x = 0;");

            Scenario.Header(HttpResponseHeaders.CacheControl).ShouldNotBeWritten();
            Scenario.Header(HttpResponseHeaders.Expires).ShouldNotBeWritten();
        }
    }
}