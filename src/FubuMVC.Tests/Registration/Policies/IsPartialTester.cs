using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class IsPartialTester
    {
        [Test]
        public void positive_case()
        {
            var chain = new BehaviorChain();
            chain.IsPartialOnly = true;

            new IsPartial().Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void negative_case()
        {
            var chain = new BehaviorChain();
            chain.IsPartialOnly = false;

            new IsPartial().Matches(chain).ShouldBeFalse();  
        }
    }
}