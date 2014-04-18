using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class passing_the_FubuMode_into_the_OWIN_host
    {
        [TearDown]
        public void TearDown()
        {
            FubuMode.Reset();
        }

        [Test]
        public void the_mode_is_passed_in()
        {
            FubuMode.Mode("ReallyRandom");
            FubuMode.Mode().ShouldEqual("ReallyRandom");

            // THIS HAS TO BE A KATANA TEST. 
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(port: 0))
            {
                server.Endpoints.Get<OwinAppModeEndpoint>(x => x.get_owin_mode())
                    .ReadAsText()
                    .ShouldEqual("reallyrandom");
            }
        }
    }

    public class OwinAppModeEndpoint
    {
        private readonly OwinContext _environment;

        public OwinAppModeEndpoint(OwinContext environment)
        {
            _environment = environment;
        }

        public string get_owin_mode()
        {
            return (_environment.Environment[OwinConstants.AppMode] as string) ?? "Not found at all";
        }
    }
}