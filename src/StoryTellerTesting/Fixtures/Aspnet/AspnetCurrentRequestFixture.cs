using System.Net;
using System.Web.Script.Serialization;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures.Aspnet
{
    public class AspnetCurrentRequestFixture : Fixture 
    {
         
        private string _lastResponse;
        private string _lastResponseContentType;
        private ExpectedResponse _expectedResponse;

        public override void SetUp(ITestContext context)
        {
            var runner = context.Retrieve<CommandRunner>();
            runner.RunFubu("packages fubu-testing -removeall");
            runner.RunFubu("restart fubu-testing");
        }

        [FormatAs("Request full {url} from request")]
        public void RequestNameWithMimeType(string url)
        {
            var client = new WebClient();
            client.Headers.Add("content-type", "application/x-www-form-urlencoded");
            client.Headers[HttpRequestHeader.Accept] = "application/json";
            _lastResponse = client.UploadString("http://localhost/fubu-testing/currentrequest/get", "FullUrl=" + url);
            _lastResponseContentType = client.ResponseHeaders["content-type"];
            _expectedResponse = new JavaScriptSerializer().Deserialize<ExpectedResponse>(_lastResponse);
        }

        [FormatAs("The full url should be {url}")]
        public string TheFullUrl()
        {
            return _expectedResponse.FullUrl;
        }

    }

    public class ExpectedResponse
    {
        public string RelativeUrl { get; set; }
        public string RawUrl { get; set; }
        public string FullUrl { get; set; }
    }
}