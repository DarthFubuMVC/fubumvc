using FubuMVC.Core.Runtime.Conditionals;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    [TestFixture]
    public class AlwaysTester
    {
        [Test]
        public void should_execute_has_to_be_true()
        {
            Always.Flyweight.ShouldExecute(null).ShouldBeTrue();
        }
    }
}