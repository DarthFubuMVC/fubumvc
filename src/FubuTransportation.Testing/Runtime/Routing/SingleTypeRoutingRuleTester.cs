using FubuTestingSupport;
using FubuTransportation.Runtime.Routing;
using NUnit.Framework;

namespace FubuTransportation.Testing.Runtime.Routing
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