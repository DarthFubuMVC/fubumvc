using System;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;

namespace FubuMVC.IntegrationTesting.Samples
{
    // SAMPLE: bootstrapping-with-katana
    public static class KatanaBootstrapper
    {
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
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded())
            {
                var greeting = server.Endpoints.Get<HelloEndpoint>(x => x.get_greeting());
                Console.WriteLine(greeting);
            }
        }
    }

    public class SimpleApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .DefaultPolicies()
                .StructureMap();
        }
    }

    public class HelloEndpoint
    {
        public string get_greeting()
        {
            return "Hello";
        }
    }
    // ENDSAMPLE



}