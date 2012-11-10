using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class IsNotPartialTester
    {
        [Test]
        public void positive_case()
        {
            var chain = new BehaviorChain();
            chain.IsPartialOnly = false;

            new IsNotPartial().Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void negative_case()
        {
            var chain = new BehaviorChain();
            chain.IsPartialOnly = true;

            new IsNotPartial().Matches(chain).ShouldBeFalse();
        }
    }
}