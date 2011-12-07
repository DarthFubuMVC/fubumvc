using System.Collections.Generic;
using System.Linq;
using FubuTestApplication;
using IntegrationTesting.Fixtures.Packages;
using Serenity;
using StoryTeller;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures.Scripts
{
    public class ScriptLoadingFixture : Fixture
    {
        private readonly NavigationDriver _navigation;

        public ScriptLoadingFixture(NavigationDriver navigation)
        {
            _navigation = navigation;
            this["SetUp"] = Embed<PackagingSetupFixture>("If the packaging setup is");
        }

        [FormatAs("Load a page that requests scripts {scriptNames}")]
        public void LoadPageWithScripts(string[] scriptNames)
        {
            var request = new ScriptRequest{
                Mandatories = scriptNames.Join(",")
            };
            _navigation.NavigateTo(request);
        }

        private IEnumerable<string> getLoadedScriptNames()
        {
            return _navigation.GetCurrentScreen().GetAssetDeclarations().Scripts.Select(x => x.GetAttribute("src").Replace("http://localhost", ""));
        }

        public IGrammar TheScriptsShouldBe()
        {
            return VerifyStringList(getLoadedScriptNames).Titled("The loaded scripts should be").Ordered().Grammar();
        }

        [FormatAs("A 'Get' to url {url} contains the text {content}")]
        public bool RequestContainsString(string url, string content)
        {
            return _navigation.GetEndpointDriver().GetHtml(url).Source().Contains(content);
        }
    }
}