using System.Net;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class writing_a_non_default_status_code
    {
        [Test]
        public void can_write_a_different_status_code()
        {
            HarnessApplication.Run(endpoints => {
                endpoints.Get<StatusCodeEndpoint>(x => x.get_not_modified()).StatusCodeShouldBe(HttpStatusCode.NotModified);
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