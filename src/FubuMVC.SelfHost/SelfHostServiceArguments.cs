using System.Net.Http;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.SelfHost
{
    public class SelfHostServiceArguments : ServiceArguments
    {
        private readonly SelfHostHttpWriter _writer;

        public SelfHostServiceArguments(RouteData routeData, HttpRequestMessage request, HttpResponseMessage response)
        {
            With(request);
            With(response);

            var cookies = new SelfHostCookies(request, response);
            With<IRequestData>(new SelfHostRequestData(routeData, request, cookies));
            With<ICurrentHttpRequest>(new SelfHostCurrentHttpRequest(request));
            With<IStreamingData>(new SelfHostStreamingData(request));
            _writer = new SelfHostHttpWriter(response);
            With<IHttpWriter>(_writer);
            With<IClientConnectivity>(new SelfHostClientConnectivity());
            With<ICookies>(cookies);
            With<IResponse>(new SelfHostResponse(response));
        }

        public SelfHostHttpWriter Writer
        {
            get { return _writer; }
        }
    }
}