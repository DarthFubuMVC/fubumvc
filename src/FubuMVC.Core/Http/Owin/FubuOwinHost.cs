using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Http.Owin.Readers;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;

namespace FubuMVC.Core.Http.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class FubuOwinHost
    {
        private readonly RouteCollection _routes;

        public static AppFunc ToAppFunc(FubuRuntime runtime, OwinSettings settings = null)
        {
            settings = settings ?? runtime.Factory.Get<OwinSettings>();
            var host = new FubuOwinHost(runtime.Routes);
            AppFunc inner = host.Invoke;
            AppFunc appFunc = settings.BuildAppFunc(inner, runtime.Factory);
            return appFunc;
        }


        public FubuOwinHost(IEnumerable<RouteBase> routes)
        {
            _routes = new RouteCollection();
            _routes.AddRange(routes);
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var owinHttpContext = new OwinHttpContext(environment);
            var routeData = _routes.GetRouteData(owinHttpContext);
            if (routeData == null)
            {
                write404(environment);
                return Task.Factory.StartNew(() => { });
            }


            new OwinRequestReader().Read(environment);

            var arguments = new OwinServiceArguments(routeData, environment);
            var invoker = routeData.RouteHandler.As<FubuRouteHandler>().Invoker;

            var taskCompletionSource = new TaskCompletionSource<object>();
            var requestCompletion = new RequestCompletion();
            requestCompletion.WhenCompleteDo(ex =>
            {
                if (ex != null)
                {
                    //This seems like OWIN should handle the .SetException(ex) with standard error screen formatting?
                    write500(environment, ex);
                }
                taskCompletionSource.SetResult(null);
            });
            arguments.With<IRequestCompletion>(requestCompletion);
            requestCompletion.SafeStart(() => invoker.Invoke(arguments, routeData.Values, requestCompletion));

            return taskCompletionSource.Task;
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
                writer.Write("FubuMVC has detected an exception\r\n");
                writer.Write(exception.ToString());
            }
        }
    }
}
