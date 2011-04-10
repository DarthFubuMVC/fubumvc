using System.Net;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures
{
    public class ConnegFixture : Fixture
    {
        private string _lastResponse;
        private string _lastResponseContentType;

        public override void SetUp(ITestContext context)
        {
            var runner = context.Retrieve<CommandRunner>();
            runner.RunFubu("packages fubu-testing -removeall");
            runner.RunFubu("restart fubu-testing");
        }


        [FormatAs("Request 'name' with {contentType}")]
        public void RequestNameWithMimeType(string contentType)
        {
            var client = new WebClient();
            client.Headers.Add("content-type", contentType);
            _lastResponse = client.DownloadString("http://localhost/fubu-testing/conneg/buckrogers");
            _lastResponseContentType = client.ResponseHeaders["content-type"];
        }

        [FormatAs("Send content {content} to mirror method with content type {contentType}")]
        public void SendToMirror(string contentType, string content)
        {
            var client = new WebClient();
            client.Headers.Add("content-type", contentType);
            _lastResponse = client.UploadString("http://localhost/fubu-testing/conneg/mirror", content);
            _lastResponseContentType = client.ResponseHeaders["content-type"];
        }

        [FormatAs("The content type of the response was {responseType}")]
        public string TheResponseContentTypeIs()
        {
            return _lastResponseContentType;
        }

        [FormatAs("The text of the response was {response}")]
        public string TheResponseText()
        {
            return _lastResponse;
        }
    }
}