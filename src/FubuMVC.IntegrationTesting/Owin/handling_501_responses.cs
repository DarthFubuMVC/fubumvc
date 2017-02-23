using System;
using System.Net;
using Xunit;

namespace FubuMVC.IntegrationTesting.Owin
{
    
    public class handling_501_responses
    {
        [Fact]
        public void handle_the_exception_with_a_501_and_the_exception_message()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<ExceptionEndpoint>(x => x.get_exception());
                _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
                _.ContentShouldContain("I did not like this");
            });
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