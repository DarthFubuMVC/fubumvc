using FubuMVC.Core.ServiceBus.Runtime.Routing;
using Shouldly;
using Xunit;
using TestMessages;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    
    public class AssemblyRuleTester
    {
        [Fact]
        public void positive_test()
        {
            var rule = AssemblyRule.For<NewUser>();
            rule.Matches(typeof(NewUser)).ShouldBeTrue();
            rule.Matches(typeof(EditUser)).ShouldBeTrue();
            rule.Matches(typeof(DeleteUser)).ShouldBeTrue();
        }

        [Fact]
        public void negative_test()
        {
            var rule = AssemblyRule.For<NewUser>();
            rule.Matches(typeof(Red.Message1)).ShouldBeFalse();
            rule.Matches(typeof(Red.Message2)).ShouldBeFalse();
            rule.Matches(GetType()).ShouldBeFalse();
        }
    }
}