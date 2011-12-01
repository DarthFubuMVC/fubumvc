using FubuMVC.Core.Diagnostics.Querying;
using OpenQA.Selenium;
using Serenity;
using StoryTeller;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures.Packages
{
    public class Package4Fixture : Fixture
    {
        private readonly RemoteBehaviorGraph _remoteGraph;
        private readonly ApplicationDriver _application;

		public Package4Fixture(RemoteBehaviorGraph remoteGraph, ApplicationDriver application)
        {
            _remoteGraph = remoteGraph;
            _application = application;

            Title = "For TestPackage4 - Spark (Assembly 'TestPackage4')";

			this["SetUp"] = Embed<PackagingSetupFixture>("If the packaging setup is");
        }

        [FormatAs("Open the browser to {url}")]
        public void OpenPage(string url)
        {
            _application.NavigateTo(url);
        }

        [FormatAs("The text of the 'message' h1 in the page should be {message}")]
        public string TextOfMessageHeadingShouldBe()
        {
            return _application.Driver.FindElement(By.Id("message")).Text;
        }
    }
}