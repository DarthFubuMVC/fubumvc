using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using Xunit;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Http.Hosting
{

    public class KatanaTester
    {
        [Fact]
        public void failing_to_host()
        {
            var exception = Exception<HostingFailedException>.ShouldBeThrownBy(() => FubuRuntime.For<BadKatanaRegistry>()).Message;
            exception.ShouldContain("65536");
            exception.ShouldContain("http");
            exception.ShouldNotContain("https");
        }
    }

    public class BadKatanaRegistry : FubuRegistry
    {
        public BadKatanaRegistry()
        {
            HostWith<Katana>(65536);
        }
    }
}
