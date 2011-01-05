using System;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller;
using StoryTeller.Engine;
using TestPackage1;

namespace IntegrationTesting.Fixtures.Packages
{
    public class Package1Fixture : Fixture
    {
        private readonly RemoteBehaviorGraph _remoteGraph;
        private readonly ApplicationDriver _application;

        public Package1Fixture(RemoteBehaviorGraph remoteGraph, ApplicationDriver application)
        {
            _remoteGraph = remoteGraph;
            _application = application;

            Title = "For TestPackage1 (Assembly 'TestPackage1')";
        }

        public IGrammar RoutesShouldBe()
        {
            return VerifySetOf(() => _remoteGraph.All().EndpointsForAssembly("TestPackage1"))
                .Titled("The routes and actions from TestPackage1 should be")
                .MatchOn(x => x.RoutePattern, x => x.FirstActionDescription);
        }

        [FormatAs("Can successfully invoke a json endpoint from this package")]
        public bool InvokingJsonService()
        {
            string name = Guid.NewGuid().ToString();
            var message = new JsonSerializedMessage(){
                Name = name
            };

            var response =
                _application.InvokeJson<JsonSerializedMessage>("http://localhost/fubu-testing/testpackage1/json/sendmessage", message);


            return response.Name == name;
        }

        [FormatAs("Can successfully invoke a string endpoint from this package")]
        public bool InvokeStringEndpoint()
        {
            return _application.InvokeString("http://localhost/fubu-testing/testpackage1/string/sayhello") == "Hello";
        }

        [FormatAs("Open the browser to {url}")]
        public void OpenPage(string url)
        {
            _application.NavigateTo(url);
        }

        [FormatAs("The text of the 'name' span in the page should be {name}")]
        public string TextOfNameElementShouldBe()
        {
            return _application.Browser.Element("name").Text;
        }
    }
}