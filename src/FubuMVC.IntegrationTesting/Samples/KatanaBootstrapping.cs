using System;
using System.Net;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Samples
{
    // SAMPLE: bootstrapping-with-katana
    public class SimpleApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .DefaultPolicies()
                .StructureMap();
        }
    }

    public static class KatanaBootstrapper
    {
        // The FubuMVC team recommends that you use an IApplicationSource
        // to contain your bootstrapping code
        public static void WithApplicationSource()
        {
            using (var server = EmbeddedFubuMvcServer.For<SimpleApplication>())
            {
                var greeting = server.Endpoints.Get<HelloEndpoint>(x => x.get_greeting());
                Console.WriteLine(greeting);
            }
        }

        public static void Inline()
        {
            // You don't have to use a custom IApplicationSource if you
            // do not want to.
            // RunEmbedded() is an extension method in FubuMVC.Katana
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded())
            {
                var greeting = server.Endpoints.Get<HelloEndpoint>(x => x.get_greeting());
                Console.WriteLine(greeting);
            }
        }
    }

    public class HelloEndpoint
    {
        public string get_greeting()
        {
            return "Hello from an Embedded FubuMVC Application";
        }
    }
    // ENDSAMPLE

    public static class KatanaSamples
    {
        public static void WithSpecialPort()
        {
            // SAMPLE: katana-with-explicit-port
            using (var server = EmbeddedFubuMvcServer.For<SimpleApplication>(port:6000))
            {
                
            }
            // ENDSAMPLE
        }

        public static void WithSpecialPath()
        {
            // SAMPLE: katana-with-explicit-path
            using (var server = EmbeddedFubuMvcServer.For<SimpleApplication>(physicalPath:"../../../SimpleApplication"))
            {

            }
            // ENDSAMPLE
        }

        public static void InsideTesting()
        {
            // SAMPLE: katana-and-testing
            using (var server = EmbeddedFubuMvcServer.For<SimpleApplication>())
            {
                // Access to the IServiceLocator for the running application
                var resolver = server.Services.GetInstance<IObjectResolver>();

                // Access to the EndpointDriver for the running application
                server.Endpoints.Get<HelloEndpoint>(x => x.get_greeting())
                    .StatusCode.ShouldEqual(HttpStatusCode.OK);

                // Access to the IUrlRegistry for the running application
                // to resolve Url's
                var url = server.Urls.UrlFor<HelloEndpoint>(x => x.get_greeting());

                // Access the FubuRuntime for this application,
                // including information about the route table that
                // this application is using
                var routes = server.Runtime.Routes;
            }
            // ENDSAMPLE
        }
    }



}