using System.Linq;
using FubuMVC.Core.Http;

namespace AspNetApplication
{
    public class RequestHeadersEndpoint
    {
        private readonly ICurrentHttpRequest _headers;

        public RequestHeadersEndpoint(ICurrentHttpRequest headers)
        {
            _headers = headers;
        }

        public string get_header_Name(HeaderRequest request)
        {
            return _headers.GetHeader(request.Name).FirstOrDefault();
        }
    }

    public class HeaderRequest
    {
        public string Name { get; set; }
    }
}