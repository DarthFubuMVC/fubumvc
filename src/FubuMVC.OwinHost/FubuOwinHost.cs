using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using Gate;
using Gate.Helpers;
using Gate.Kayak;
using Kayak;

namespace FubuMVC.OwinHost
{
    

    // TODO -- Deal with recycling.  Look at the SchedulerDelegate
    public class FubuOwinHost
    {
        private readonly IApplicationSource _source;
        private readonly ISchedulerDelegate _scheduler;
        private FubuRuntime _runtime;

        public FubuOwinHost(IApplicationSource source, ISchedulerDelegate scheduler)
        {
            _source = source;
            _scheduler = scheduler;
        }

        public void RunApplication(int port)
        {
            _runtime = _source.BuildApplication().Bootstrap();
            
            var ep = new IPEndPoint(IPAddress.Any, 5500);
            Console.WriteLine("Listening on " + ep);
            KayakGate.Start(_scheduler, ep, builder =>
            {
                builder
                    .RescheduleCallbacks()
                    .Run(ExecuteRequest);
            });
        }

        public void ExecuteRequest(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
        {
            var request = new Request(env);
            var response = new Response(result);

            Console.Write("Received " + request.Path);

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

            Console.WriteLine(" ({0})", response.Status);
        }

        private void write404(Response response)
        {
            response.Status = "404";
            response.Write("Sorry, I can't find this resource");
        }

        private void executeRoute(Request request, RouteData routeData, Response response, Action<Exception> fault)
        {
            var arguments = new OwinServiceArguments(routeData, request, response);

            // TODO -- maybe smuggle behaviorId out a different way here
            var behaviorId = routeData.RouteHandler.As<FubuRouteHandler>().BehaviorId;
            var behavior = _runtime.Factory.BuildBehavior(arguments, behaviorId);

            try
            {
                behavior.Invoke();
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

        private FubuRuntime buildRuntime()
        {
            RouteTable.Routes.Clear();
            var application = _source.BuildApplication();

            // TODO -- blow up if any non-FubuRouteHandler's in the system?
            return application.Bootstrap();
        }


    }

    // TODO -- this gets pluggable at some point
}