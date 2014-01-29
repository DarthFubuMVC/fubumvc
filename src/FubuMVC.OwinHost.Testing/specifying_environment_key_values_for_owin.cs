using FubuCore;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class specifying_environment_key_values_for_owin
    {
        [Test]
        public void can_inject_environment_keys_from_the_fubu_registry()
        {
            using (var server = FubuApplication
                .For<OverriddenEnvironmentRegistry>()
                .StructureMap()
                .RunEmbedded(autoFindPort: true))
            {
                server.Endpoints.GetByInput(new KeyRequest {Key = "Foo"})
                    .ReadAsText().ShouldEqual("1");

                server.Endpoints.GetByInput(new KeyRequest { Key = "Bar" })
                    .ReadAsText().ShouldEqual("2");
            }
        }
    }

    public class OverriddenEnvironmentRegistry : FubuRegistry
    {
        public OverriddenEnvironmentRegistry()
        {
            AlterSettings<OwinSettings>(x => {
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