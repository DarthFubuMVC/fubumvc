using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Security.Authorization;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class InputOutputNodePlacementIntegrationTester
    {
        private BehaviorGraph theGraph;

        public InputOutputNodePlacementIntegrationTester()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<PlacementController>();

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Fact]
        public void input_is_first_even_with_authorization()
        {
            theGraph.ChainFor<PlacementController>(x => x.post_payload(null))
                .Top.ShouldBeOfType<InputNode>()
                .Next.ShouldBeOfType<AuthorizationNode>();
        }

        [Fact]
        public void output_is_last()
        {
            theGraph.ChainFor<PlacementController>(x => x.post_payload(null))
                .Last().ShouldBeOfType<OutputNode>();
        }
    }

    public class PlacementController
    {
        public string SayHello()
        {
            return "hello";
        }

        [AllowRole("some role")]
        public PlacementPayload post_payload(PlacementPayload payload)
        {
            return payload;
        } 
    }

    public class PlacementPayload
    {
        
    }
}