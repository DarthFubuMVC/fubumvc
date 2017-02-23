using FubuMVC.Core.Projections;
using FubuMVC.Core.Runtime;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Projections
{
    
    public class ValueSourceTester
    {
        [Fact]
        public void find_values_invokes_the_fubu_request()
        {
            var request = new InMemoryFubuRequest();
            var address = new Address();

            request.Set(address);

            var source = new ValueSource<Address>(request);

            source.FindValues().ShouldBeOfType<SimpleValues<Address>>()
                .Subject.ShouldBeTheSameAs(address);
        }
    }
}