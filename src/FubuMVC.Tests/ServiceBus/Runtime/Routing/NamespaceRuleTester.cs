using FubuMVC.Core.ServiceBus.Runtime.Routing;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Routing
{
    [TestFixture]
    public class NamespaceRuleTester
    {
        [Test]
        public void positive_test()
        {
            var rule = NamespaceRule.For<Red.Message1>();
            rule.Matches(typeof(Red.Message1)).ShouldBeTrue();
            rule.Matches(typeof(Red.Message2)).ShouldBeTrue();
            rule.Matches(typeof(Red.Message3)).ShouldBeTrue();
        }

        [Test]
        public void negative_test()
        {
            var rule = NamespaceRule.For<Red.Message1>();
            rule.Matches(typeof(Green.Message1)).ShouldBeFalse();
            rule.Matches(typeof(Green.Message2)).ShouldBeFalse();
            rule.Matches(typeof(Green.Message3)).ShouldBeFalse();
        }
    }
}

namespace Red
{
    public class Message1{}
    public class Message2{}
    public class Message3{}

}

namespace Green
{

    public class Message1 { }
    public class Message2 { }
    public class Message3 { }
}