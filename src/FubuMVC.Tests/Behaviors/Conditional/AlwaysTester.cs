using FubuMVC.Core.Runtime.Conditionals;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    
    public class AlwaysTester
    {
        [Fact]
        public void should_execute_has_to_be_true()
        {
            Always.Flyweight.ShouldExecute(null).ShouldBeTrue();
        }
    }
}