using FubuMVC.Core.ServiceBus.Runtime.Routing;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    [TestFixture]
    public class SingleTypeRoutingRuleTester
    {
        [Test]
        public void positive_match()
        {
            new SingleTypeRoutingRule<BusSettings>().Matches(typeof(BusSettings))
                                                    .ShouldBeTrue();
        }

        [Test]
        public void negative_match()
        {
            new SingleTypeRoutingRule<BusSettings>().Matches(GetType())
                                                    .ShouldBeFalse();
        }
    }
}