using FubuMVC.Core.ServiceBus.Runtime.Routing;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    [TestFixture]
    public class LambdaRoutingRuleTester
    {
        [Test]
        public void positive_match()
        {
            var rule = new LambdaRoutingRule(type => type == typeof (BusSettings));
            rule.Matches(typeof(BusSettings)).ShouldBeTrue();
        }

        [Test]
        public void negative_match()
        {
            var rule = new LambdaRoutingRule(type => type == typeof(BusSettings));
            rule.Matches(GetType()).ShouldBeFalse();
        }
    }
}