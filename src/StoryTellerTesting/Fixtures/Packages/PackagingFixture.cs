using System.Linq;
using System.Net;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller.Assertions;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures.Packages
{
    public class PackagingFixture : Fixture
    {
        private readonly RemoteBehaviorGraph _remoteGraph;
        private readonly CommandRunner _runner;

        public PackagingFixture(RemoteBehaviorGraph remoteGraph, CommandRunner runner)
        {
            _remoteGraph = remoteGraph;
            _runner = runner;


            this["SetUp"] = Embed<PackagingSetupFixture>("If the packaging setup is");        
        }

        [FormatAs("No endpoints registered for assembly {assemblyName}")]
        public bool NoActionsForAssembly(string assemblyName)
        {
            return !_remoteGraph.All().EndpointsForAssembly(assemblyName).Any();
        }

        [FormatAs("Some endpoints are registered for assembly {assemblyName}")]
        public bool ActionsForAssemblyArePresent(string assemblyName)
        {
            return _remoteGraph.All().EndpointsForAssembly(assemblyName).Any();
        }

        [FormatAs("The url for image '{name}' should be {url}")]
        public string ImageUrlFor(string name)
        {
            return _remoteGraph.GetImageUrl(name);
        }

        [FormatAs("Downloading image '{name}' is successful with a mime type of {mimeType}")]
        public string DownloadImage(string name)
        {
            var url = _remoteGraph.GetImageUrl(name);
            var client = new WebClient();
            var bytes = client.DownloadData("http://localhost/" + url);
            
            StoryTellerAssert.Fail(bytes.Length < 500, "Not enough data detected for the image.  Did it really load?");

            return client.ResponseHeaders[HttpResponseHeader.ContentType];
        }
    }
}