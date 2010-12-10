using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using FubuFastPack.StructureMap;
using System.Linq;

namespace FubuFastPack.Testing
{
    [TestFixture]
    public class ContainerActivatorExtensionsTester
    {
        [Test]
        public void run_activator_from_container()
        {
            var service = MockRepository.GenerateMock<IService>();
            var container = new Container(x =>
            {
                x.For<IService>().Use(service);
                x.Activate<IService>("the description", s => s.Do("this"));
            });

            var activator = container.GetAllInstances<IActivator>().Single();
            activator.ToString().ShouldEqual("the description");

            activator.Activate(null, null);

            service.AssertWasCalled(x => x.Do("this"));
        }
    }

    public interface IService
    {
        void Do(string name);
    }
}