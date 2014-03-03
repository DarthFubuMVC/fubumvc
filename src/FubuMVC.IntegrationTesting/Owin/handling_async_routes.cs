using System;
using System.Net;
using System.Threading.Tasks;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    public class handling_async_routes
    {
        [Test]
        public void async_route_should_render()
        {
            var response = Harness.Endpoints.Get<AsyncEndpoint>(x => x.get_async()).ReadAsText();
            response.ShouldEqual("Hello");
        } 

        [Test]
        public void async_route_with_error_should_have_501_and_exception_message()
        {
            var response = Harness.Endpoints.Get<AsyncEndpoint>(x => x.get_async_error());
            response.StatusCode.ShouldEqual(HttpStatusCode.InternalServerError);
            var text = response.ReadAsText();
            text.ShouldContain("Error in async method");
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
            return Task<string>.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Error in async method");
            });
        }
    }
}