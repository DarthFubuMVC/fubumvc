using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class asset_writing_headers_are_configurable : AssetIntegrationContext
    {
        public asset_writing_headers_are_configurable()
        {
            FubuMode.Reset();
            File("Foo.js").WriteLine("var x = 0;");
        }

        public class MyRegistry : FubuRegistry
        {
            public MyRegistry()
            {
                AlterSettings<AssetSettings>(x => {
                    x.Headers[HttpResponseHeaders.CacheControl] = () => "Foo!";
                    x.Headers[HttpResponseHeaders.Expires] = () => "Bar!";
                });
            }
        }


        [Test]
        public void writes_the_headers_and_content()
        {
            Scenario.Get.Url("/Foo.js");

            Scenario.ContentShouldContain("var x = 0;");

            Scenario.Header(HttpResponseHeaders.CacheControl).SingleValueShouldEqual("Foo!");
            Scenario.Header(HttpResponseHeaders.Expires).SingleValueShouldEqual("Bar!");
        }
    }
}