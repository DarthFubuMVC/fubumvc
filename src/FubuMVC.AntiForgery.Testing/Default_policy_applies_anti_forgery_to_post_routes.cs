using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class Default_policy_applies_anti_forgery_to_post_routes
    {
        [Test]
        public void the_order_of_the_configuration_action_was_wrong()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<TestEndpoint>();
            using (var runtime = FubuApplication.For(registry).ContainerFacility(new StructureMapContainerFacility(new Container())).Bootstrap())
            {
                var graph = runtime.Factory.Get<BehaviorGraph>();

                graph.BehaviorFor<TestEndpoint>(x => x.post_csrf(null))
                    .OfType<AntiForgeryNode>().Any()
                    .ShouldBeTrue();
            }


        }
    }

    public class TestEndpoint
    {
        public HtmlTag post_csrf(PostRequest request)
        {
            return new HtmlTag("h1", h => h.Text("POST HOLA"));
        }

        public HtmlTag get_csrf(GetRequest request)
        {
            return new HtmlTag("h1", h => h.Text("GET HOLA"));
        }
    }

    public class GetResult
    {
    }

    public class GetRequest
    {
    }

    public class PostRequest
    {
    }

    public class PostResult
    {
    }
}