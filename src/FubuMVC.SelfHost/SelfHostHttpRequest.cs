using System.Net.Http;
using System.Web;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpRequest : HttpRequestBase
    {
        private readonly HttpRequestMessage _request;

        public SelfHostHttpRequest(HttpRequestMessage request)
        {
            _request = request;
        }

        public override string PathInfo
        {
            get
            {
                return _request.RequestUri.AbsolutePath.TrimStart('/');
            }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~/"; }
        }

        public override string HttpMethod
        {
            get { return _request.Method.Method; }
        }
    }
}