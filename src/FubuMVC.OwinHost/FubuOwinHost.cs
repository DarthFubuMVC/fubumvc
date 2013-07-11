using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;
using FubuMVC.OwinHost.Readers;

namespace FubuMVC.OwinHost
{
    public class FubuOwinHost
    {
        private readonly RouteCollection _routes;

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
            requestCompletion.SafeStart(() => invoker.Invoke(arguments, routeData.Values, requestCompletion));

            return taskCompletionSource.Task;
        }

        private void write404(IDictionary<string, object> environment)
        {
            environment[OwinConstants.ResponseStatusCodeKey] = HttpStatusCode.NotFound;
        }

        private void write500(IDictionary<string, object> environment, Exception exception)
        {
            environment[OwinConstants.ResponseStatusCodeKey] = HttpStatusCode.InternalServerError;
            var response = environment.Get<Stream>(OwinConstants.ResponseBodyKey);
            using (var writer = new StreamWriter(response))
            {
                writer.Write("FubuMVC has detected an exception\r\n");
                writer.Write(exception.ToString());
            }
        }
    }
}
