using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Runtime;
using Gate;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class FubuOwinHost
    {
        public bool Verbose { get; set; }

        public void ExecuteRequest(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
        {
            var request = new Request(env);
            var response = new Response(result);

            if (Verbose) Console.WriteLine("Received " + request.Path);

            var context = new GateHttpContext(request);
            var routeData = RouteTable.Routes.GetRouteData(context);

            if (routeData == null)
            {
                // TODO -- try to do it by mapping the files
                write404(response);
            }
            else
            {
                executeRoute(request, routeData, response, fault);
            }

            // TODO -- return 404 if the route is not found

            if (Verbose) Console.WriteLine(" ({0})", response.Status);
        }

        private static void write404(Response response)
        {
            response.Status = "404";
            response.Write("Sorry, I can't find this resource");
        }

        private static void executeRoute(Request request, RouteData routeData, Response response, Action<Exception> fault)
        {
            var arguments = new OwinServiceArguments(routeData, request, response);

            var invoker = routeData.RouteHandler.As<FubuRouteHandler>().Invoker;

            try
            {
                invoker.Invoke(arguments, routeData.Values);
            }
            catch (Exception ex)
            {
                response.Status = "500";
                response.Write("FubuMVC has detected an exception\r\n");
                response.Write(ex.ToString());
            }
            finally
            {
                response.Finish();
            }
        }
    }
}