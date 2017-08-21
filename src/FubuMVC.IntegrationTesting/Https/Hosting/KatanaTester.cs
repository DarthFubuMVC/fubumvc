using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using Xunit;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Http.Hosting
{

    public class KatanaTesterHttps
    {
        [Fact]
        public void failing_to_host()
        {
            var exception = Exception<HostingFailedException>.ShouldBeThrownBy(() => FubuRuntime.For<BadKatanaRegistryHttps>());
            exception.Message.ShouldContain("65537");
            exception.Message.ShouldContain("https");
        }
    }

    public class BadKatanaRegistryHttps : FubuRegistry
    {
        public BadKatanaRegistryHttps()
        {
            HostWith<Katana>(65537, true);
        }
    }
}
