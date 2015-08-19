using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http.Hosting;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    // This little endpoint class handles
    // the root url of the application
    public class HomeEndpoint
    {
        public string Index()
        {
            return "Hello, World";
        }
    }

    [TestFixture]
    public class RunHelloWorld
    {
        [Test]
        public void start_and_run()
        {
            // Bootstrap a basic FubuMVC applications
            // with just the default policies and services
            using (var runtime = FubuRuntime.Basic())
            {
                // Execute the home route and verify
                // the response
                runtime.Scenario(_ =>
                {
                    _.Get.Url("/");

                    _.StatusCodeShouldBeOk();
                    _.ContentShouldBe("Hello, World");
                });
            }
        }
    }

    public class ExampleRegistry : FubuRegistry
    {
        public ExampleRegistry()
        {
            // Turn on some opt in features
            Features.Localization.Enable(true);
            Features.Diagnostics.Enable(TraceLevel.Production);

            // Change the application mode if you want
            Mode = "development";

            // Have the application use an embedded 
            // Katana host at port 5501
            HostWith<Katana>(5501);

            // Register services with the IoC container
            // using a superset of StructureMap's
            // Registry DSL
            Services.AddService<IActivator, MyActivator>();
        
            // For testing purposes, you may want 
            // to bootstrap the application from an external
            // testing library, in which case, you'd want
            // to override where FubuMVC looks for static
            // asset files like JS or CSS files
            UseParallelDirectory("MyApp");
            // or
            RootPath = "some other path";
        }
    }

    public static class BootstrappingExample
    {
        public static void Start()
        {
            using (var server = FubuRuntime.For<ExampleRegistry>())
            {
                // do stuff with the application
                // or wait for some kind of signal
                // that you should shut it off
            }




            var runtime = FubuRuntime.Basic(_ =>
            {
                _.Features.Diagnostics.Enable(TraceLevel.Verbose);

                // I'm opting for NOWIN hosting this time
                // and letting FubuMVC try to pick an open
                // port starting from 5500
                _.HostWith<NOWIN>();
            });
        }
    }

    public class MyActivator : IActivator
    {
        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            throw new System.NotImplementedException();
        }
    }

}