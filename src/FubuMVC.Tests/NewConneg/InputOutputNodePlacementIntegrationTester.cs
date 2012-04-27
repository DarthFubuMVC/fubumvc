using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Security;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class InputOutputNodePlacementIntegrationTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<PlacementController>();

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void input_is_first_even_with_authorization()
        {
            theGraph.BehaviorFor<PlacementController>(x => x.post_payload(null))
                .Top.ShouldBeOfType<InputNode>()
                .Next.ShouldBeOfType<AuthorizationNode>();
        }

        [Test]
        public void output_is_last()
        {
            theGraph.BehaviorFor<PlacementController>(x => x.post_payload(null))
                .Last().ShouldBeOfType<OutputNode>();
        }
    }

    public class PlacementController
    {
        public string SayHello()
        {
            return "hello";
        }

        [AllowRole("some role"), Conneg]
        public PlacementPayload post_payload(PlacementPayload payload)
        {
            return payload;
        } 
    }

    public class PlacementPayload
    {
        
    }
}