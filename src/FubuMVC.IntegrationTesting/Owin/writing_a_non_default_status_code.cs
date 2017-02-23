using System.Net;
using FubuMVC.Core.Runtime;
using Xunit;

namespace FubuMVC.IntegrationTesting.Owin
{
    
    public class writing_a_non_default_status_code
    {
        [Fact]
        public void can_write_a_different_status_code()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<StatusCodeEndpoint>(x => x.get_not_modified());
                _.StatusCodeShouldBe(HttpStatusCode.NotModified);
            });
        }
    }

    public class StatusCodeEndpoint
    {
        private readonly IOutputWriter _writer;

        public StatusCodeEndpoint(IOutputWriter writer)
        {
            _writer = writer;
        }

        public string get_not_modified()
        {
            _writer.WriteResponseCode(HttpStatusCode.NotModified, "No changes here");

            return "Nothing to see here";
        }
    }
}