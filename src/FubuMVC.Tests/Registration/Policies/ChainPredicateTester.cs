using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class ChainPredicateTester
    {
        private ChainPredicate thePredicate;
        private IChainFilter filter1;
        private IChainFilter filter2;
        private IChainFilter filter3;
        private IChainFilter filter4;
        private BehaviorChain aChain;

        [SetUp]
        public void SetUp()
        {
            thePredicate = new ChainPredicate();

            filter1 = MockRepository.GenerateMock<IChainFilter>();
            filter2 = MockRepository.GenerateMock<IChainFilter>();
            filter3 = MockRepository.GenerateMock<IChainFilter>();
            filter4 = MockRepository.GenerateMock<IChainFilter>();

            thePredicate.Matching(filter1);
            thePredicate.Matching(filter2);
            thePredicate.Matching(filter3);
            thePredicate.Matching(filter4);

            aChain = new BehaviorChain();
        }

        [Test]
        public void does_not_pass_if_there_are_no_filters()
        {
            var predicate = new ChainPredicate();
            predicate.As<IChainFilter>().Matches(new BehaviorChain())
                .ShouldBeFalse();
        }

        [Test]
        public void does_not_match_if_none_of_the_internal_filters_match()
        {
            thePredicate.As<IChainFilter>().Matches(aChain).ShouldBeFalse();
        }

        [Test]
        public void matches_if_any_internals_match_1()
        {
            filter3.Stub(x => x.Matches(aChain)).Return(true);

            thePredicate.As<IChainFilter>().Matches(aChain).ShouldBeTrue();
        }

        [Test]
        public void matches_if_any_internals_match_2()
        {
            filter1.Stub(x => x.Matches(aChain)).Return(true);

            thePredicate.As<IChainFilter>().Matches(aChain).ShouldBeTrue();
        }
    }
}