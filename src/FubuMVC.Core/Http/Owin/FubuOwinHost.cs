using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Http.Owin.Readers;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;
using HtmlTags;

namespace FubuMVC.Core.Http.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class FubuOwinHost
    {
        private readonly RouteCollection _routes;

        public static AppFunc ToAppFunc(FubuRuntime runtime, OwinSettings settings = null)
        {
            settings = settings ?? runtime.Get<OwinSettings>();
            var host = new FubuOwinHost(runtime.Routes);
            AppFunc inner = host.Invoke;
            AppFunc appFunc = settings.BuildAppFunc(inner, runtime.Get<IServiceFactory>());

            var diagnostics = runtime.Get<DiagnosticsSettings>();
            return diagnostics.WrapAppFunc(runtime, appFunc);
        }


        public FubuOwinHost(IEnumerable<RouteBase> routes)
        {
            _routes = new RouteCollection();
            _routes.AddRange(routes);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var owinHttpContext = new OwinHttpContext(environment);
            var routeData = _routes.GetRouteData(owinHttpContext);
            if (routeData == null)
            {
                write404(environment);
                return;
            }


            new OwinRequestReader().Read(environment);

            var arguments = new OwinServiceArguments(routeData, environment);
            var invoker = routeData.RouteHandler.As<FubuRouteHandler>().Invoker;

            try
            {
                await invoker.Invoke(arguments, routeData.Values).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                write500(environment, e);
            }
        }

        private void write404(IDictionary<string, object> environment)
        {
            environment[OwinConstants.ResponseStatusCodeKey] = HttpStatusCode.NotFound;
        }

        private void write500(IDictionary<string, object> environment, Exception exception)
        {
            using (var writer = new OwinHttpResponse(environment))
            {
                writer.WriteResponseCode(HttpStatusCode.InternalServerError);
                var document = new HtmlDocument
                {
                    Title = "Exception!"
                };

                document.Add("h1").Text("FubuMVC has detected an exception");
                document.Add("hr");
                document.Add("pre").Id("error").Text(exception.ToString());

                writer.WriteContentType(MimeType.Html.Value);
                writer.Write(document.ToString());
            }
        }
    }
}
