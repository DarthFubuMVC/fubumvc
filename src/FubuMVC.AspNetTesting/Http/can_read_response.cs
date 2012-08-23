using System.Diagnostics;
using AspNetApplication.Http;
using FubuMVC.Core.Http.Headers;
using NUnit.Framework;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.AspNetTesting.Http
{
    [TestFixture]
    public class can_read_response
    {
        [Test]
        public void read_values_from_the_response()
        {
            var request = new AspNetRequest{
                Headers = new Header[]{new Header("x_a", "1"), new Header("x_b", "2")},
                StatusCode = 201,
                StatusDescription = "Weird"
            };

            var response = TestApplication.Endpoints.PostJson(request).ReadAsJson<AspNetResponse>();
            response.Description.ShouldEqual(request.StatusDescription);
            response.StatusCode.ShouldEqual(request.StatusCode);

            request.Headers.Each(x =>
            {
                response.ResponseHeaders.ShouldContain(x);
            });
        }
    }
}