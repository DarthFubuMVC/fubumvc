using FubuMVC.Core;
using FubuMVC.StructureMap;

namespace QuickStart
{
    // SAMPLE: hello-world-application
    public class HelloWorldApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.DefaultPolicies().StructureMap();
        }
    }
    // ENDSAMPLE

    // SAMPLE: hello-world-home
    public class HomeEndpoint
    {
        public string Index()
        {
            return "Hello World!";
        }
    }
    // ENDSAMPLE
}