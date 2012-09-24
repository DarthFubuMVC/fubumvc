using System.Net.Http;
using System.Web.Http.SelfHost;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.SelfHost
{
    public class SelfHostServiceArguments : ServiceArguments
    {
        private readonly SelfHostHttpWriter _writer;

        public SelfHostServiceArguments(RouteData routeData, HttpRequestMessage request, HttpResponseMessage response, HttpSelfHostConfiguration configuration)
        {
            With(request);
            With(response);

            With<IRequestData>(new SelfHostRequestData(routeData, request));
            With<ICurrentHttpRequest>(new SelfHostCurrentHttpRequest(request, configuration));
            With<IStreamingData>(new SelfHostStreamingData(request));
            _writer = new SelfHostHttpWriter(response);
            With<IHttpWriter>(_writer);
            With<IClientConnectivity>(new SelfHostClientConnectivity());
            With<ICookies>(new SelfHostCookies(request, response));
            With<IResponse>(new SelfHostResponse(response));
        }

        public SelfHostHttpWriter Writer
        {
            get { return _writer; }
        }
    }
}