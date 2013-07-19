using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Autofac.Testing.Compliance
{
    [TestFixture]
    public class IServiceFactory_Compliance
    {
        [Test]
        public void has_the_IServiceFactory_registered()
        {
            ContainerFacilitySource.New(x => { })
                                   .Get<IServiceFactory>().ShouldBeOfType<AutofacContainerFacility>();
        }
    }
}