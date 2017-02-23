using FubuMVC.Core.ServiceBus.Runtime.Routing;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    
    public class LambdaRoutingRuleTester
    {
        [Fact]
        public void positive_match()
        {
            var rule = new LambdaRoutingRule(type => type == typeof (BusSettings));
            rule.Matches(typeof(BusSettings)).ShouldBeTrue();
        }

        [Fact]
        public void negative_match()
        {
            var rule = new LambdaRoutingRule(type => type == typeof(BusSettings));
            rule.Matches(GetType()).ShouldBeFalse();
        }
    }
}