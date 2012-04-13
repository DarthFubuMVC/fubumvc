using FubuMVC.Core.Behaviors.Conditional;
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
            Always.Flyweight.ShouldExecute().ShouldBeTrue();
        }
    }
}