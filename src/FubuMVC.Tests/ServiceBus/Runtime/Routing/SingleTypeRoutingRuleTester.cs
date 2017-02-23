using FubuMVC.Core.ServiceBus.Runtime.Routing;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    
    public class SingleTypeRoutingRuleTester
    {
        [Fact]
        public void positive_match()
        {
            new SingleTypeRoutingRule<BusSettings>().Matches(typeof(BusSettings))
                                                    .ShouldBeTrue();
        }

        [Fact]
        public void negative_match()
        {
            new SingleTypeRoutingRule<BusSettings>().Matches(GetType())
                                                    .ShouldBeFalse();
        }
    }
}