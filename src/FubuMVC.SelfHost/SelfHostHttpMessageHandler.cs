using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Handlers;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpMessageHandler : HttpMessageHandler
    {
        private readonly FubuRuntime _runtime;
        private readonly RouteCollection _routes;

        public SelfHostHttpMessageHandler(FubuRuntime runtime)
        {
            _runtime = runtime;

            _routes = new RouteCollection();
            _routes.AddRange(runtime.Routes);

            RouteTable.Routes.Clear();
        }

        private RouteData determineRouteData(HttpRequestMessage request)
        {
            var context = new SelfHostHttpContext(request);
            return _routes.GetRouteData(context);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => ExecuteRequest(request), cancellationToken);
        }

        public bool Verbose { get; set; }

        public HttpResponseMessage ExecuteRequest(HttpRequestMessage request)
        {
            if (Verbose) Console.WriteLine("Received {0} - {1}", request.Method, request.RequestUri);

            var routeData = determineRouteData(request);

            if (routeData == null)
            {
                return write404();
            }
            else
            {
                try
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var arguments = new SelfHostServiceArguments(routeData, request, response);
                    var invoker = routeData.RouteHandler.As<FubuRouteHandler>().Invoker;
                    invoker.Invoke(arguments, routeData.Values);

                    arguments.Writer.AttachContent();

                    return response;
                }
                catch (Exception ex)
                {
                    return write500(ex);
                }
            }
        }

        private static HttpResponseMessage write500(Exception ex)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError){
                Content = new StringContent("FubuMVC has detected an exception\r\n" + ex.ToString())
            };
        }

        private static HttpResponseMessage write404()
        {
            return new HttpResponseMessage{
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Sorry, I can't find this resource")
            };
        }
    }
}