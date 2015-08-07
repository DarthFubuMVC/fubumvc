using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class specifying_environment_key_values_for_owin
    {
        [Test]
        public void can_inject_environment_keys_from_the_fubu_registry()
        {
            using (var server = FubuRuntime
                .For<OverriddenEnvironmentRegistry>())
            {
                using (var client = new WebClient())
                {
                    client.DownloadString(server.BaseAddress.AppendUrl("/environment/Foo")).ShouldBe("1");
                    client.DownloadString(server.BaseAddress.AppendUrl("/environment/Bar")).ShouldBe("2");
                }
            }
        }
    }

    public class OverriddenEnvironmentRegistry : FubuRegistry
    {
        public OverriddenEnvironmentRegistry()
        {
            HostWith<Katana>();

            AlterSettings<OwinSettings>(x =>
            {
                x.EnvironmentData["Foo"] = "1";
                x.EnvironmentData["Bar"] = "2";

                x.Properties.Add("foo.Bar", "3");
            });
        }
    }

    public class OwinEnvironmentEndpoint
    {
        private readonly OwinContext _context;

        public OwinEnvironmentEndpoint(OwinContext context)
        {
            _context = context;
        }

        public string get_environment_Key(KeyRequest request)
        {
            return _context.Environment.Get<string>(request.Key) ?? "not found";
        }
    }

    public class KeyRequest
    {
        public string Key { get; set; }
    }
}