using System.Linq;
using FubuCore;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using SpecificationExtensions = FubuTestingSupport.SpecificationExtensions;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class OutputBeforeAjaxContinuationPolicyTester
    {
        [Test]
        public void reorders_output_node_before_the_first_action()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<AjaxController>();
            });

            var chain = graph.BehaviorFor<AjaxController>(x => x.get_success());
            chain.First().ShouldBeOfType<OutputNode>();
            chain.Last().ShouldBeOfType<ActionCall>();
        }

        [Test]
        public void modifies_a_chain()
        {
            var chain = new BehaviorChain();
            var theAction = ActionCall.For<AjaxController>(x => x.get_success());
            chain.AddToEnd(theAction);
            chain.AddToEnd(chain.Output.As<OutputNode>());

            OutputBeforeAjaxContinuationPolicy.Modify(chain);

            chain.First().ShouldBeTheSameAs(chain.Output);
            chain.Last().ShouldBeTheSameAs(theAction);
        }

        [Test]
        public void chains_with_input()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<AjaxController>();
            });

            var chain = graph.BehaviorFor<AjaxController>(x => x.get_with_input(null));
            var nodes = chain.ToList();
            nodes[0].ShouldBeOfType<InputNode>();
            nodes[1].ShouldBeOfType<OutputNode>();
            nodes.Last().ShouldBeOfType<ActionCall>();
        }

        public class AjaxController
        {
            public AjaxContinuation get_success()
            {
                return AjaxContinuation.Successful();
            }

            public AjaxContinuation get_with_input(AjaxControllerInput input)
            {
                return AjaxContinuation.Successful();
            }

            public AjaxContinuation get_with_failures()
            {
                return new AjaxContinuation{
                    Success = false,
                    Message = "You stink!"
                };
            }
        }

        public class AjaxControllerInput { }
    }
}