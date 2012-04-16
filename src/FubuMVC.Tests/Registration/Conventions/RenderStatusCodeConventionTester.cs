using System.Diagnostics;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;
using RestfulStatusCodeServices;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RenderStatusCodeConventionTester
    {
        [Test]
        public void render_status_behavior_is_attached_to_chains_that_return_a_status_code()
        {
            var graph = new RestfulServicesRegistry().BuildGraph();
            var outputs = graph.BehaviorFor<RestfulService>(x => x.Action1(null)).Outputs;


            outputs.OfType<OutputNode>().Single().BehaviorType.ShouldEqual(typeof (RenderStatusCodeBehavior));
        }

        [Test]
        public void look_at_our_application()
        {
            var registry = new RestfulServicesRegistry();
            var writer 
                = new BehaviorGraphWriter(registry.BuildGraph(), null, null, new StubCurrentHttpRequest());

            Debug.WriteLine(writer.PrintRoutes());
        }
    }


    public class RestfulServicesRegistry : FubuRegistry
    {
        public RestfulServicesRegistry()
        {
            // Need to tell FubuMVC which classes are actions
            Actions.IncludeTypes(x => x.IsInNamespace("RestfulStatusCodeServices"));

            // Tell FubuMVC to toss the RenderStatusCodeBehavior on
            // the back of any behavior chain where the action returns
            // an HttpStatusCode
            Output.ToBehavior<RenderStatusCodeBehavior>()
                .WhenTheOutputModelIs<HttpStatusCode>();

            Routes
                .ConstrainToHttpMethod(x => x.OutputType() == typeof(HttpStatusCode), "POST")
                .IgnoreControllerNamespaceEntirely();
        }
    }
}

namespace RestfulStatusCodeServices
{
    // JsonMessage is just a marker interface
    // FubuMVC doesn't yet support "conneg" -- but should
    // We take pull requests!
    public class InputMessage : JsonMessage {}

    public class RestfulService
    {
        public HttpStatusCode Action1(InputMessage message)
        {
            // perform the action
            return HttpStatusCode.OK;
        }
    }

}