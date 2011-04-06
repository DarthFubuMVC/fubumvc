using FubuFastPack.Crud;
using FubuFastPack.Testing.Security;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class EnsureEntityExistsConventionTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<SitesController>();
                x.Actions.IncludeType<CasesController>();
            
                x.ApplyConvention<EnsureEntityExistsConvention>();
            });

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void should_place_an_EnsureEntityExistsBehavior_in_front_of_entity_endpoints()
        {
            var chain = theGraph.BehaviorFor<SitesController>(x => x.Edit(null));
            chain.FirstCall().Previous.ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(
                typeof (EnsureEntityExistsBehavior<Site>));
        }

        [Test]
        public void should_place_an_EnsureEntityExistsBehavior_in_front_of_entity_endpoints_2()
        {
            var chain = theGraph.BehaviorFor<CasesController>(x => x.Edit(null));
            chain.FirstCall().Previous.ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(
                typeof(EnsureEntityExistsBehavior<Case>));
        }

        [Test]
        public void should_not_place_a_wrapper_around_non_entity_endpoints()
        {
            theGraph.BehaviorFor<SitesController>(x => x.New()).Any(x => x is Wrapper).ShouldBeFalse();
        }
    }
}