using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
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
            TestHost.Scenario(_ => {
                _.Get.Action<RedirectedEndpoint>(x => x.get_redirect());

                _.StatusCodeShouldBe(HttpStatusCode.Redirect);
                _.Header(HttpResponseHeaders.Location).SingleValueShouldEqual("/redirect/correct");
                _.ContentShouldContain("The document has moved");
            });
        }

        [Test]
        public void FubuContinuation_Redirect_honors_the_explicit_METHOD()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<RedirectedEndpoint>(x => x.get_redirect_explicit());

                _.StatusCodeShouldBe(HttpStatusCode.Redirect);
                _.Header(HttpResponseHeaders.Location).SingleValueShouldEqual("/redirect/wrong");
                _.ContentShouldContain("The document has moved");
            });
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