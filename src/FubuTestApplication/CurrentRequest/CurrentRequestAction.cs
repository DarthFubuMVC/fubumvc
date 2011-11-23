using FubuMVC.Core;
using FubuMVC.Core.Http;

namespace FubuTestApplication.CurrentRequest
{
    public class CurrentRequestAction
    {
        private readonly ICurrentHttpRequest _currentHttpRequest;

        public CurrentRequestAction(ICurrentHttpRequest currentHttpRequest)
        {
            _currentHttpRequest = currentHttpRequest;
        }

        public UrlContextModel Get(UrlRequest request)
        {
            return new UrlContextModel()
                       {
                           RelativeUrl = _currentHttpRequest.RelativeUrl(),
                           RawUrl = _currentHttpRequest.RawUrl(),
                           FullUrl = _currentHttpRequest.ToFullUrl(request.FullUrl ?? "/")
                       };


        }
    }

    public class UrlRequest
    {
        public string FullUrl { get; set; }
    }

    public class UrlContextModel : JsonMessage 
    {
        public string RelativeUrl { get; set; }

        public string RawUrl { get; set; }

        public string FullUrl { get; set; }
    }
}