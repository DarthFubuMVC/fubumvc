using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    [TestFixture]
    public class loading_content_and_actions_from_a_linked_package : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"
init src/TestPackage1 pak1
link src/FubuMVC.Core.Assets.IntegrationTesting pak1
");
        }

        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ScriptsHandler>();
        }

        [Test]
        public void reads_asset_config_from_the_bottle()
        {
            var request = new ScriptRequest
            {
                Mandatories = "Pak1Set"
            };

            endpoints.GetByInput(request).ScriptNames()
                .ShouldHaveTheSameElementsAs(
                "_content/scripts/Pak1-A.js",
                "_content/scripts/Script1.js",
                "_content/scripts/Script2.js"
                );
        }
    }

}