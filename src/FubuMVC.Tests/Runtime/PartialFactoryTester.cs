using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class PartialFactoryTester : InteractionContext<PartialFactory>
    {
        private BehaviorGraph graph;
        private ServiceArguments args;

        protected override void beforeEach()
        {
            graph = new BehaviorGraph(null);
            Services.Inject(graph);
            args = new ServiceArguments();
            Services.Inject(args);
        }

        [Test]
        public void for_an_action_call()
        {
            BehaviorChain chain1 = BehaviorChain.For<ControllerTarget>(x => x.OneInOneOut(null));
            graph.AddChain(chain1);

            BehaviorChain chain2 = BehaviorChain.For<ControllerTarget>(x => x.OneInZeroOut(null));
            graph.AddChain(chain2);


            var behavior = MockRepository.GenerateMock<IActionBehavior>();
            MockFor<IBehaviorFactory>()
                .Stub(x => x.BuildBehavior(args, chain1.UniqueId))
                .Return(behavior);

            ClassUnderTest.BuildPartial(ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null)))
                .ShouldBeOfType<PartialCurrentChainSwitcher>().Inner
                .ShouldBeTheSameAs(behavior);
        }

        [Test]
        public void for_an_input_type()
        {
            BehaviorChain chain1 = BehaviorChain.For<ControllerTarget>(x => x.OneInOneOut(null));
            graph.AddChain(chain1);

            var behavior = MockRepository.GenerateMock<IActionBehavior>();
            MockFor<IBehaviorFactory>()
                .Stub(x => x.BuildBehavior(args, chain1.UniqueId))
                .Return(behavior);

            ClassUnderTest.BuildPartial(typeof (Model1))
                .ShouldBeOfType<PartialCurrentChainSwitcher>().Inner
                .ShouldBeTheSameAs(behavior);
        }
    }
}