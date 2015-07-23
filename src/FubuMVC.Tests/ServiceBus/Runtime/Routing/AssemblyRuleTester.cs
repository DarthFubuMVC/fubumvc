using FubuMVC.Core.ServiceBus.Runtime.Routing;
using Shouldly;
using NUnit.Framework;
using TestMessages;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    [TestFixture]
    public class AssemblyRuleTester
    {
        [Test]
        public void positive_test()
        {
            var rule = AssemblyRule.For<NewUser>();
            rule.Matches(typeof(NewUser)).ShouldBeTrue();
            rule.Matches(typeof(EditUser)).ShouldBeTrue();
            rule.Matches(typeof(DeleteUser)).ShouldBeTrue();
        }

        [Test]
        public void negative_test()
        {
            var rule = AssemblyRule.For<NewUser>();
            rule.Matches(typeof(Red.Message1)).ShouldBeFalse();
            rule.Matches(typeof(Red.Message2)).ShouldBeFalse();
            rule.Matches(GetType()).ShouldBeFalse();
        }
    }
}