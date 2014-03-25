using System.Runtime.InteropServices;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class asset_writing_in_production : AssetIntegrationContext
    {
        public asset_writing_in_production()
        {
            FubuMode.Reset();
            File("Foo.js").WriteLine("var x = 0;");
        }

        [Test]
        public void writes_the_headers_and_content()
        {
            Scenario.Get.Url("/Foo.js");

            Scenario.ContentShouldContain("var x = 0;");

            Scenario.Header(HttpResponseHeaders.CacheControl).SingleValueShouldEqual("private, max-age=86400");
            Scenario.Header(HttpResponseHeaders.Expires).ShouldHaveOneNonNullValue();
        }
    }
}