using System.Net;
using FubuMVC.Core.Endpoints;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class SelfHostHttpServerTester : SharedHarnessContext
    {
        [Test]
        public void can_make_request_after_recycle()
        {
            makeRequest().StatusCodeShouldBe(HttpStatusCode.OK);
            SelfHostHarness.Recycle();
            makeRequest().StatusCodeShouldBe(HttpStatusCode.OK);
        }

        private HttpResponse makeRequest()
        {
            return endpoints.Get<TestSelfHostHttpServerEndpoints>(x => x.get_success());
        }

        public class TestSelfHostHttpServerEndpoints
        {
            public string get_success()
            {
                return "success";
            }
        }
    }
}