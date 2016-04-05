using FubuMVC.Core;

namespace AssemblyPackage
{
    public class HomeEndpoint
    {
        public string Index()
        {
            return "Hello, World";
        }
    }

    public class SampleApp : FubuRegistry
    {
        
    }

    public static class RunStuff
    {
        public static void do_the_home_page()
        {
            using (var runtime = FubuRuntime.For<SampleApp>())
            {
                runtime.Scenario(_ =>
                {
                    _.Get.Url("");
                    _.ContentShouldBe("Hello, World");
                    
                });
            }
        }
    }
}