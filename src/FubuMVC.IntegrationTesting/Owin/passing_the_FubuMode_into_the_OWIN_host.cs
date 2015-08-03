using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class passing_the_FubuMode_into_the_OWIN_host
    {
        [Test]
        public void the_mode_is_passed_in()
        {
            // THIS HAS TO BE A KATANA TEST. 
            using (var server = FubuRuntime.Basic(_ =>
            {
                _.Mode = "ReallyRandom";
                _.HostWith<Katana>();
            }))
            {
                server.Endpoints.Get<OwinAppModeEndpoint>(x => x.get_owin_mode())
                    .ReadAsText()
                    .ShouldBe("reallyrandom");
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