using FubuMVC.Core;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class PartialOnlyConventionTester
    {
        [Test]
        public void ShouldBePartial_positive()
        {
            var call = ActionCall.For<PartialController>(x => x.GoPartial(null));
            PartialOnlyConvention.ShouldBePartial(call).ShouldBeTrue();
        }

        [Test]
        public void ShouldBePartial_negative()
        {
            var call = ActionCall.For<PartialController>(x => x.Go(null));
            PartialOnlyConvention.ShouldBePartial(call).ShouldBeFalse();
        }

        [Test]
        public void integrated_with_fubu_registry()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<PartialController>();

            var graph = registry.BuildGraph();

            graph.BehaviorFor<PartialController>(x => x.Go(null)).IsPartialOnly.ShouldBeFalse();
            graph.BehaviorFor<PartialController>(x => x.GoPartial(null)).IsPartialOnly.ShouldBeTrue();
        }


        public class Input1{}

        public class PartialController
        {
            public void Go(Input1 input){}
            public void GoPartial(Input1 input){}
        }


    }
}