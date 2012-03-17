using System.Net;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwnHttpWriterTester
    {
        private Response response;
        private OwinHttpWriter writer;

        [SetUp]
        protected void beforeEach()
        {
            response = new Response(null);
            writer = new OwinHttpWriter(response);
        }

        [Test]
        public void should_set_response_code()
        {
            writer.WriteResponseCode(HttpStatusCode.UseProxy);

            response.Status.ShouldEqual("305");
        }

        [Test]
        public void should_set_response_code_and_description()
        {
            const string description = "why u no make good request?";
            writer.WriteResponseCode(HttpStatusCode.BadRequest, description);
            response.Status.ShouldEqual("400 {0}".ToFormat(description));
        }
    }
}