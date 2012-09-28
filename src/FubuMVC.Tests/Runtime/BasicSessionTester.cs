using FubuMVC.Core.Runtime;
using FubuMVC.Tests.NewConneg;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class BasicSessionTester
    {
        [Test]
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