using System.Net.Http;
using System.Web;
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

            var httpRequest = new SelfHostCurrentHttpRequest(request);
            With<ICurrentHttpRequest>(httpRequest);

            With<IRequestData>(new SelfHostRequestData(routeData, request, httpRequest));

            With<IStreamingData>(new SelfHostStreamingData(request));
            _writer = new SelfHostHttpWriter(response, httpRequest);
            With<IHttpWriter>(_writer);
            With<IClientConnectivity>(new SelfHostClientConnectivity());
            With<IResponse>(new SelfHostResponse(response));

            With<HttpContextBase>(new SelfHostHttpContext(request));
        }

        public SelfHostHttpWriter Writer
        {
            get { return _writer; }
        }
    }
}