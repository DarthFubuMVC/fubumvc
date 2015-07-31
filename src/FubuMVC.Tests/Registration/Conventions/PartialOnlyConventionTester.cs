using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class PartialOnlyConventionTester
    {

        [Test]
        public void integrated_with_fubu_registry()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<PartialController>();

            var graph = BehaviorGraph.BuildFrom(registry);

            graph.ChainFor<PartialController>(x => x.Go(null)).IsPartialOnly.ShouldBeFalse();
            graph.ChainFor<PartialController>(x => x.GoPartial(null)).IsPartialOnly.ShouldBeTrue();
        }


        public class Input1{}

        public class PartialController
        {
            public void Go(Input1 input){}
            public void GoPartial(Input1 input){}
        }


    }
}