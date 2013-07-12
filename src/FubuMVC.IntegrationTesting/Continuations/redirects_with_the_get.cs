using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Continuations
{
    [TestFixture]
    public class redirects_with_the_get
    {
        [Test]
        public void the_FubuContinuation_Redirect_uses_GET_by_default()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded(port: 5510))
            {
                server.Endpoints.Get<RedirectedEndpoint>(x => x.get_redirect())
                      .ReadAsText().ShouldEqual("Right!");

            }
        }

        [Test]
        public void FubuContinuation_Redirect_honors_the_explicit_METHOD()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded(port: 5510))
            {
                server.Endpoints.Get<RedirectedEndpoint>(x => x.get_redirect_explicit())
                      .ReadAsText().ShouldNotEqual("Right!");

            }
        }
    }

    public class RedirectRequest
    {
        
    }

    public class RedirectedEndpoint
    {
        public FubuContinuation get_redirect()
        {
            return FubuContinuation.RedirectTo<RedirectRequest>();
        }

        public FubuContinuation get_redirect_explicit()
        {
            return FubuContinuation.RedirectTo<RedirectRequest>("POST");
        }

        public string get_redirect_correct(RedirectRequest request)
        {
            return "Right!";
        }

        public string post_redirect_wrong(RedirectRequest request)
        {
            return "Wrong!";
        }
    }
}