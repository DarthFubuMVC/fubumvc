using System.Collections.Generic;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Endpoints;
using FubuMVC.IntegrationTesting.Conneg;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class writing_asset_tags_with_and_without_bottles : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles("link harness --clean-all");
            runFubu("packages harness --clean-all --remove-all");
        }

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
                .ShouldHaveHeader(HttpResponseHeader.LastModified)
                .ShouldHaveHeader(HttpResponseHeader.CacheControl);
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
link harness pak1
");

            restart();

            LoadPageWithScripts("Pak1-A.js").ScriptNames()
                .ShouldHaveTheSameElementsAs("_content/scripts/Pak1-A.js");


        
        }
    }
}