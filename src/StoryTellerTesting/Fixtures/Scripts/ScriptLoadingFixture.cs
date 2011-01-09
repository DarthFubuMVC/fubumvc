using System.Collections.Generic;
using IntegrationTesting.Fixtures.Packages;
using StoryTeller;
using StoryTeller.Engine;
using WatiN.Core;
using WatiN.Core.Constraints;
using System.Linq;

namespace IntegrationTesting.Fixtures.Scripts
{
    public class ScriptLoadingFixture : Fixture
    {
        private readonly ApplicationDriver _application;

        public ScriptLoadingFixture(ApplicationDriver application)
        {
            _application = application;
            this["SetUp"] = Embed<PackagingSetupFixture>("If the packaging setup is");
        }

        [FormatAs("Load a page that requests scripts {scriptNames}")]
        public void LoadPageWithScripts(string[] scriptNames)
        {
            var url = "http://localhost/fubu-testing/scriptloading/" + scriptNames.Join(",");
            _application.NavigateTo(url);
        }

        private IEnumerable<string> getLoadedScriptNames()
        {
            return _application.Browser.ElementsWithTag("script").Select(x => x.GetAttributeValue("src"));
        }

        public IGrammar TheScriptsShouldBe()
        {
            return VerifyStringList(getLoadedScriptNames).Titled("The loaded scripts should be").Ordered().Grammar();
        }

        [FormatAs("A 'Get' to url {url} contains the text {content}")]
        public bool RequestContainsString(string url, string content)
        {
            return _application.InvokeString(url).Contains(content);
        }
    }
}