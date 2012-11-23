using FubuMVC.Core.Http;

namespace AspNetApplication
{
    public class RequestHeadersEndpoint
    {
        private readonly IRequestHeaders _headers;

        public RequestHeadersEndpoint(IRequestHeaders headers)
        {
            _headers = headers;
        }

        public string get_header_Name(HeaderRequest request)
        {
            string text = null;
            _headers.Value<string>(request.Name, x => text = x);

            return text;
        }
    }

    public class HeaderRequest
    {
        public string Name { get; set; }
    }
}