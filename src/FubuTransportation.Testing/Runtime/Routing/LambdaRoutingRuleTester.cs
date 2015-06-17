using FubuTransportation.Runtime.Routing;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuTransportation.Testing.Runtime.Routing
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