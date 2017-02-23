using System;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Owin
{
    public class handling_async_routes
    {
        [Fact]
        public void async_route_should_render()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AsyncEndpoint>(x => x.get_async());
                _.ContentShouldBe("Hello");
            });
        }

        [Fact]
        public void async_route_with_error_should_have_501_and_exception_message()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AsyncEndpoint>(x => x.get_async_error());

                _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
                _.ContentShouldContain("Error in async method");
            });
        }
    }

    public class AsyncEndpoint
    {
        public Task<string> get_async()
        {
            return Task<string>.Factory.StartNew(() => "Hello");
        }

        public Task<string> get_async_error()
        {
            return Task<string>.Factory.StartNew(() => { throw new InvalidOperationException("Error in async method"); });
        }
    }
}