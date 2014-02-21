using System;
using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using System.Linq;
using System.Collections.Generic;

namespace AspNetApplication.Http
{
    public class ResponseController
    {
        private readonly IOutputWriter _writer;
        private readonly IHttpResponse _response;

        public ResponseController(IOutputWriter writer, IHttpResponse response)
        {
            _writer = writer;
            _response = response;
        }

        public AspNetResponse post_fake_response(AspNetRequest request)
        {
            _writer.WriteResponseCode((HttpStatusCode) Enum.ToObject(typeof(HttpStatusCode), request.StatusCode), request.StatusDescription);
        
            if (request.Headers != null)
            {
                request.Headers.Each(x => x.Write(_writer));
            }

            return new AspNetResponse{
                Description = _response.StatusDescription,
                StatusCode = _response.StatusCode,
                ResponseHeaders = _response.AllHeaders().ToArray()
            };
        }        
    }

    public class AspNetRequest
    {
        public int StatusCode { get; set;}
        public string StatusDescription { get; set;}
        public Header[] Headers { get; set; }
    }

    public class AspNetResponse
    {
        public Header[] ResponseHeaders { get; set; }
        public int StatusCode { get; set; }
        public string Description { get; set; }
    }
}