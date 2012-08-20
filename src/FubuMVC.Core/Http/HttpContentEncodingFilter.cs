using System.Net;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    public class HttpContentEncodingFilter : IBehaviorInvocationFilter
    {
        public DoNext Filter(ServiceArguments arguments)
        {
            var headers = arguments.Get<IRequestHeaders>();
            headers.Value<string>(HttpRequestHeader.AcceptEncoding, x =>
            {
                var encoding = arguments.Get<IHttpContentEncoders>().MatchFor(x);
                arguments.Get<IHttpWriter>().UseEncoding(encoding);
            });

            return DoNext.Continue;
        }
    }
}