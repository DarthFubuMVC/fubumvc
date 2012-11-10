using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class OrChainFilterTester
    {
        private OrChainFilter theFilter;
        private IChainFilter filter1;
        private IChainFilter filter2;
        private IChainFilter filter3;
        private IChainFilter filter4;
        private BehaviorChain aChain;

        [SetUp]
        public void SetUp()
        {
            theFilter = new OrChainFilter();

            filter1 = MockRepository.GenerateMock<IChainFilter>();
            filter2 = MockRepository.GenerateMock<IChainFilter>();
            filter3 = MockRepository.GenerateMock<IChainFilter>();
            filter4 = MockRepository.GenerateMock<IChainFilter>();

            theFilter.Add(filter1);
            theFilter.Add(filter2);
            theFilter.Add(filter3);
            theFilter.Add(filter4);

            aChain = new BehaviorChain();
        }

        [Test]
        public void does_not_match_if_none_of_the_internal_filters_match()
        {
            theFilter.Matches(aChain).ShouldBeFalse();
        }

        [Test]
        public void matches_if_any_internals_match_1()
        {
            filter3.Stub(x => x.Matches(aChain)).Return(true);

            theFilter.Matches(aChain).ShouldBeTrue();
        }

        [Test]
        public void matches_if_any_internals_match_2()
        {
            filter1.Stub(x => x.Matches(aChain)).Return(true);

            theFilter.Matches(aChain).ShouldBeTrue();
        }
    }
}