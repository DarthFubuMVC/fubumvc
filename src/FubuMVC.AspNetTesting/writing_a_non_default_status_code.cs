using System.Net;
using AspNetApplication;
using FubuMVC.IntegrationTesting;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class writing_a_non_default_status_code
    {
        [Test]
        public void can_write_a_different_status_code()
        {
            TestApplication.Endpoints.Get<StatusCodeEndpoint>(x => x.get_not_modified())
                .StatusCodeShouldBe(HttpStatusCode.NotModified);
        }
    }
}