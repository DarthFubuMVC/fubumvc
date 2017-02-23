using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class PartialRegistrationConventionTester
    {
        private BehaviorGraph theGraph = BehaviorGraph.BuildFrom(x =>
        {
            x.Actions.IncludeType<PartialsController>()
                .IncludeType<SomePartialsController>();
        });

        [Fact]
        public void actions_from_a_handler_class_marked_with_the_FubuPartial_attribute()
        {
            theGraph.ChainFor<PartialsController>(x => x.A(null)).IsPartialOnly.ShouldBeTrue();
            theGraph.ChainFor<PartialsController>(x => x.B(null)).IsPartialOnly.ShouldBeTrue();
            theGraph.ChainFor<PartialsController>(x => x.C(null)).IsPartialOnly.ShouldBeTrue();
        }

        [Fact]
        public void actions_marked_with_FubuPartial_should_be_partial_only()
        {
            theGraph.ChainFor<SomePartialsController>(x => x.A(null)).IsPartialOnly.ShouldBeTrue();
        }

        [Fact]
        public void actions_not_marked_with_FubuPartial_are_not_partials()
        {
            theGraph.ChainFor<SomePartialsController>(x => x.B(null)).IsPartialOnly.ShouldBeFalse();
            theGraph.ChainFor<SomePartialsController>(x => x.C(null)).IsPartialOnly.ShouldBeFalse();
        }


        [FubuPartial]
        public class PartialsController
        {
            public void A(Model1 model){}
            public void B(Model1 model){}
            public void C(Model1 model){}
        }

        
        public class SomePartialsController
        {
            [FubuPartial]
            public void A(Model1 model) { }
            public void B(Model1 model) { }
            public void C(Model1 model) { }
        }
    }
}