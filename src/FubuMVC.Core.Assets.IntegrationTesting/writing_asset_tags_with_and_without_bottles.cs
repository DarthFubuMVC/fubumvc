using System.Collections.Generic;
using System.Net;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Runtime;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    [TestFixture]
    public class writing_asset_tags_with_and_without_bottles : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ScriptsHandler>();
        }

        public HttpResponse LoadPageWithScripts(params string[] scriptNames)
        {
            var request = new ScriptRequest
            {
                Mandatories = scriptNames.Join(",")
            };
            return endpoints.GetByInput(request);
        }

        [Test]
        public void asset_tags_should_have_cache_headers_set()
        {
            endpoints.GetAsset(AssetFolder.scripts, "Script1.js")
                .ShouldHaveHeader(HttpResponseHeader.ETag)
                .ShouldHaveHeader(HttpResponseHeader.CacheControl);
        }

        [Test]
        public void can_read_an_asset_from_the_main_application_on_the_first_read()
        {
            endpoints.GetAsset(AssetFolder.scripts, "Script1.js")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentTypeShouldBe(MimeType.Javascript)
                .ReadAsText().ShouldContain("var x = 3;");
        }

        [Test]
        public void read_asset_with_etag_should_return_NotModified_and_no_content()
        {
            // First request without an etag gets the whole thing
            var etag = endpoints.GetAsset(AssetFolder.scripts, "Script1.js")
                .ContentTypeShouldBe(MimeType.Javascript)
                .StatusCodeShouldBe(HttpStatusCode.OK).Etag();

            // Subsequent requests with etag
            endpoints.GetAsset(AssetFolder.scripts, "Script1.js", etag: etag)
                .StatusCodeShouldBe(HttpStatusCode.NotModified);

            endpoints.GetAsset(AssetFolder.scripts, "Script1.js", etag: etag)
                .StatusCodeShouldBe(HttpStatusCode.NotModified)
                .ContentLength().ShouldBeLessThan(200);

            endpoints.GetAsset(AssetFolder.scripts, "Script1.js", etag: etag)
                .StatusCodeShouldBe(HttpStatusCode.NotModified)
                .ShouldHaveHeader(HttpResponseHeader.CacheControl)
                .ShouldHaveHeader(HttpResponseHeader.ETag);
        }

        [Test]
        public void write_script_tags()
        {
            LoadPageWithScripts("Script1.js", "Script2.js", "Script3.js")
                .ScriptNames()
                .ShouldHaveTheSameElementsAs("_content/scripts/Script1.js", "_content/scripts/Script2.js", "_content/scripts/Script3.js");
        
        
        
            // Load with duplicates
            LoadPageWithScripts("Script1.js", "Script2.js", "Script1.js", "Script2.js")
                .ScriptNames()
                .ShouldHaveTheSameElementsAs("_content/scripts/Script1.js", "_content/scripts/Script2.js");
        
        
            // Load a set
            LoadPageWithScripts("OneToFour").ScriptNames()
                .ShouldHaveTheSameElementsAs("_content/scripts/Script1.js", "_content/scripts/Script2.js", "_content/scripts/Script3.js", "_content/scripts/Script4.js");

            runBottles(@"
init src/TestPackage1 pak1
link src/FubuMVC.Core.Assets.IntegrationTesting pak1
");

            restart();

            LoadPageWithScripts("Pak1-A.js").ScriptNames()
                .ShouldHaveTheSameElementsAs("_content/scripts/Pak1-A.js");


        
        }
    }
}