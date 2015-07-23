using System;
using System.Net;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class handling_501_responses
    {
        [Test]
        public void handle_the_exception_with_a_501_and_the_exception_message()
        {
            var response = Harness.Endpoints.Get<ExceptionEndpoint>(x => x.get_exception());

            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            var text = response.ReadAsText();

            text.ShouldContain("I did not like this");
        }
    }

    public class ExceptionEndpoint
    {
        public string get_exception()
        {
            throw new ApplicationException("I did not like this");
        }
    }
}