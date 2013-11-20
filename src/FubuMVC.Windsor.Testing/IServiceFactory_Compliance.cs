using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Windsor.Testing
{
    [TestFixture]
    public class IServiceFactory_Compliance
    {
        [Test]
        public void has_the_IServiceFactory_registered()
        {
            ContainerFacilitySource.New(x => { })
                                   .Get<IServiceFactory>().ShouldBeOfType<WindsorContainerFacility>();
        }
    }
}