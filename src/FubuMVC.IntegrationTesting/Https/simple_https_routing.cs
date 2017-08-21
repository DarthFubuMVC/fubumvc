using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using Xunit;

namespace FubuMVC.IntegrationTesting.Https
{
    public class simple_https_routing
    {
        [Fact]
        public void start_and_run()
        {
            using (var runtime = FubuRuntime.For<SimpleRegistry>())
            {
                runtime.Scenario(_ =>
                {
                    _.Request.FullUrl("https://127.0.0.1/simple-action");
                    _.StatusCodeShouldBeOk();
                    _.ContentShouldBe("Simple Action");
                    _.ContentTypeShouldBe("text/plain");
                });
            }
        }
    }

    public class SimpleRegistry : FubuRegistry
    {
        public SimpleRegistry()
        {
            Actions.DisableDefaultActionSource();
            Actions.IncludeType<SimpleEndpoint>();
            HostWith<Katana>(5501, true);
        }
    }

    public class SimpleEndpoint
    {
        [UrlPattern("simple-action")]
        public string Route()
        {
            return "Simple Action";
        }
    }
}
