using FubuMVC.Core.Runtime;
using FubuMVC.Tests.UI;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Runtime
{
    
    public class BasicSessionTester
    {
        [Fact]
        public void store_and_retrieve_objects()
        {
            var address = new Address();
            var model = new AddressViewModel();

            var session = new BasicSessionState();
            session.Set(address);
            session.Set(model);

            session.Get<Address>().ShouldBeTheSameAs(address);
            session.Get<AddressViewModel>().ShouldBeTheSameAs(model);
        }
    }
}