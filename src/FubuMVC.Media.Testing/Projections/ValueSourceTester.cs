using FubuMVC.Core.Runtime;
using FubuMVC.Media.Projections;
using FubuMVC.Media.Testing.Xml;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Media.Testing.Projections
{
    [TestFixture]
    public class ValueSourceTester
    {
        [Test]
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