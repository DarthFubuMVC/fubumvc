using FubuCore;
using FubuFastPack.Binding;
using FubuFastPack.Persistence;
using FubuFastPack.StructureMap;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using FubuMVC.StructureMap;

namespace FubuFastPack.Testing
{
    [TestFixture]
    public class RegistrationTester
    {
        [Test]
        public void fastpack_registry_is_overridden()
        {
            var container = new Container(x =>
            {
                x.For<IRepository>().Use<InMemoryRepository>();
                x.For<IEntityFinder>().Use<EntityFinder>();
                x.For<IEntityFinder>().Use<EntityFinder>();
                x.For<IEntityFinderCache>().Use<StructureMapEntityFinderCache>();
            });

            FubuApplication
                .For<FubuRegistry>()
                .StructureMap(() => container)
                .Packages(x => x.Assembly(typeof(FastPackObjectConverter).Assembly))
                .Bootstrap();

            container.GetInstance<IObjectConverter>().ShouldBeOfType<FastPackObjectConverter>();
        }
    }
}