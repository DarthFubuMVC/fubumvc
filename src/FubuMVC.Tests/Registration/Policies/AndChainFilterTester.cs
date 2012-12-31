using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class AndChainFilterTester
    {
        private IChainFilter f1;
        private IChainFilter f2;
        private IChainFilter f3;
        private BehaviorChain chain;
        private AndChainFilter filter;

        [SetUp]
        public void SetUp()
        {
            f1 = MockRepository.GenerateMock<IChainFilter>();
            f2 = MockRepository.GenerateMock<IChainFilter>();
            f3 = MockRepository.GenerateMock<IChainFilter>();

            chain = new BehaviorChain();

            filter = new AndChainFilter(f1, f2, f3);            
        }

        [Test]
        public void negative_when_none_match()
        {
            filter.Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void negative_if_any_fail()
        {
            f2.Stub(x => x.Matches(chain)).Return(true);
            f3.Stub(x => x.Matches(chain)).Return(true);

            filter.Matches(chain).ShouldBeFalse();
        }


        [Test]
        public void negative_if_any_fail_2()
        {
            f1.Stub(x => x.Matches(chain)).Return(true);
            f2.Stub(x => x.Matches(chain)).Return(true);

            filter.Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void positive_if_all_match()
        {
            f1.Stub(x => x.Matches(chain)).Return(true);
            f2.Stub(x => x.Matches(chain)).Return(true);
            f3.Stub(x => x.Matches(chain)).Return(true);

            filter.Matches(chain).ShouldBeTrue();
        }
    }
}