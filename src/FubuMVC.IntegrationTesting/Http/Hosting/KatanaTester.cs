using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Http.Hosting
{
    [TestFixture]
    public class KatanaTester
    {
        [Test]
        public void failing_to_host()
        {
            Exception<HostingFailedException>.ShouldBeThrownBy(() => FubuRuntime.For<BadKatanaRegistry>()).Message.ShouldContain("65536");
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