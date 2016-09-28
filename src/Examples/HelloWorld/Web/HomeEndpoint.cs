using System;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using Xunit;

namespace Examples.HelloWorld.Web
{
    // SAMPLE: HelloWorld-HomeEndpoint
    public class HomeEndpoint
    {
        public string Index()
        {
            return "Hello, world!";
        }
    }
    // ENDSAMPLE


    public class RunHelloWorld
    {
        
        public void bootstrap()
        {
            // SAMPLE: HelloWorld-Bootstrapping
            using (var runtime = FubuRuntime.Basic())
            {
                
            } 
            // ENDSAMPLE   
        }

        // SAMPLE: HelloWorld-Running
        [Fact]
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
        // ENDSAMPLE
    }

    // SAMPLE: HelloWorld-self-host
    public static class Program
    {
        public static int Main(string[] args)
        {
            // Bootstrap the same basic application, but this time
            // add NOWIN web server hosting
            using (var runtime = FubuRuntime.Basic(_ => _.HostWith<NOWIN>()))
            {
                Console.WriteLine("Web application available at " + runtime.BaseAddress);
            }

            return 0;
        }
    }
    // ENDSAMPLE
}