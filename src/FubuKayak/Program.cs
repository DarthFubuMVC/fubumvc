using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.OwinHost;
using Gate;
using Gate.Helpers;
using Gate.Kayak;
using HtmlTags;
using Kayak;

namespace FubuKayak
{
    class Program
    {
        static void Main(string[] args)
        {
            RouteTable.Routes.Add(new Route("what/is/it", new FakeHandler()));
            RouteTable.Routes.Add(new Route("my/name/{is}", new FakeHandler()));


            var ep = new IPEndPoint(IPAddress.Any, 5500);
            Console.WriteLine("Listening on " + ep);
            KayakGate.Start(new SchedulerDelegate(), ep, Startup.Configuration);
        }
    }

    public class FakeHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return null;
        }
    }

    public class Startup
    {
        // called automatically when Kayak starts up.
        public static void Configuration(IAppBuilder builder)
        {
            // we'll create a very simple pipeline:

            builder
                // insert a middleware between Kayak and Nancy
                // to ensure Kayak objects are only accessed 
                // on Kayak's worker thread.
                .RescheduleCallbacks()

                // cap off the pipeline with Nancy
                .Run(ExecuteRequest);
        }

        public static void ExecuteRequest(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
        {
            var request = new Request(env);
            var context = new GateHttpContext(request);

            var document = new HtmlDocument();

            document.Add("h1").Text("Path:  " + request.Path);

            var data = RouteTable.Routes.GetRouteData(context);
            if (data != null)
            {
                document.Add("p").Text("Route is " + data.Route.As<Route>().Url);
                
                
                if (data.Values != null)
                {
                    document.Push("table");
                    data.Values.Each(pair =>
                    {
                        document.Push("tr");
                        document.Add("td").Text(pair.Key);
                        document.Add("td").Text(pair.Value == null ? string.Empty : pair.Value.ToString());
                        document.Pop();
                        
                    });

                    document.Pop();
                }
            }

            var response = new Response(result);



            response.ContentType = "text/html";
            response.Write(document.ToString());

            response.Finish();
        }
    }

    

    public class SchedulerDelegate : ISchedulerDelegate
    {
        public void OnException(IScheduler scheduler, Exception e)
        {
            // called whenever an exception occurs on Kayak's event loop.
            // this is good place for logging. here's a start:
            Console.WriteLine("Exception on scheduler");
            Console.Out.WriteStackTrace(e);
        }

        public void OnStop(IScheduler scheduler)
        {
            // called when Kayak's run loop is about to exit.
            // this is a good place for doing clean-up or other chores.
            Console.WriteLine("Scheduler is stopping.");
        }
    }
}
