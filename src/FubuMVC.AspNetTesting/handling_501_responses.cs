using System.Net;
using AspNetApplication;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class handling_501_responses
    {
        [Test]
        public void handle_the_exception_with_a_501_and_the_exception_message()
        {
            var response = TestApplication.Endpoints.Get<ExceptionEndpoint>(x => x.get_exception());

            response.StatusCode.ShouldEqual(HttpStatusCode.InternalServerError);
            var text = response.ReadAsText();

            text.ShouldContain("I did not like this");
        }
    }
}