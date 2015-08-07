using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture]
    public class reading_and_writing_http_headers_and_responses_with_OWIN
    {
        [Test]
        public void read_values_from_the_response()
        {
            var request = new OwinRequest
            {
                Headers = new[] {new Header("x_a", "1"), new Header("x_b", "2")},
                StatusCode = 201,
                StatusDescription = "Weird"
            };

            TestHost.Scenario(_ =>
            {
                _.Post.Json(request);

                _.StatusCodeShouldBe(HttpStatusCode.Created);

                var response = _.Response.Body.ReadAsJson<OwinResponse>();
                response.Description.ShouldBe(request.StatusDescription);
                response.StatusCode.ShouldBe(request.StatusCode);

                request.Headers.Each(x => { response.ResponseHeaders.ShouldContain(x); });
            });
        }
    }

    public class ResponseEndpoints
    {
        private readonly IOutputWriter _writer;
        private readonly IHttpResponse _response;

        public ResponseEndpoints(IOutputWriter writer, IHttpResponse response)
        {
            _writer = writer;
            _response = response;
        }

        public OwinResponse post_fake_response(OwinRequest request)
        {
            _writer.WriteResponseCode((HttpStatusCode) Enum.ToObject(typeof (HttpStatusCode), request.StatusCode),
                request.StatusDescription);

            if (request.Headers != null)
            {
                request.Headers.Each(x => x.Write(_writer));
            }

            return new OwinResponse
            {
                Description = _response.StatusDescription,
                StatusCode = _response.StatusCode,
                ResponseHeaders = _response.AllHeaders().ToArray()
            };
        }
    }

    public class OwinRequest
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public Header[] Headers { get; set; }
    }

    public class OwinResponse
    {
        public Header[] ResponseHeaders { get; set; }
        public int StatusCode { get; set; }
        public string Description { get; set; }
    }
}