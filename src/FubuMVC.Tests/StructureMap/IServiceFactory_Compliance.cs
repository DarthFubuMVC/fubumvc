using FubuMVC.Core.Runtime;
using FubuMVC.Core.StructureMap;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.StructureMap
{
    [TestFixture]
    public class IServiceFactory_Compliance
    {
        [Test]
        public void has_the_IServiceFactory_registered()
        {
            ContainerFacilitySource.New(x => { })
                                   .Get<IServiceFactory>().ShouldBeOfType<StructureMapContainerFacility>();
        }
    }
}