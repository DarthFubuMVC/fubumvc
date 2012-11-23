using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.StructureMap.Testing.Compliance
{
    [TestFixture]
    public class ObjectDef_Compliance
    {
        [Test]
        public void simple_ObjectDef_by_type()
        {
            var container = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
            });

            container.Get<IService>().ShouldBeOfType<SimpleService>();
        }
    }

    public interface IService
    {
        
    }

    public class SimpleService : IService
    {
        
    }

    
}