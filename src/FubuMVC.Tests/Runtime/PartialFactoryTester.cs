using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration;
using FubuMVC.Tests.TestSupport;
using Shouldly;
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
            graph = new BehaviorGraph();
            Services.Inject(graph);
            args = new ServiceArguments();
            Services.Inject(args);
        }


    }
}