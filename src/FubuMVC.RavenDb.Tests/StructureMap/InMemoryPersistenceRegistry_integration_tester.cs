using System;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Reset;
using FubuMVC.RavenDb.Storage;
using NUnit.Framework;
using Shouldly;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.RavenDb.Tests.StructureMap
{
    [TestFixture]
    public class InMemoryPersistenceRegistry_integration_tester
    {
        private IContainer theContainer;

        [SetUp]
        public void SetUp()
        {
            theContainer = new Container(x => x.AddRegistry<TestRegistry>());
        }

        private IEntityRepository theRepository
        {
            get { return theContainer.GetInstance<IEntityRepository>(); }
        }


        [Test]
        public void can_persist_and_retrieve_an_entities()
        {
            var entity = new FakeEntity {Id = Guid.NewGuid()};

            theRepository.Update(entity);

            theRepository.Find<FakeEntity>(entity.Id).ShouldBeTheSameAs(entity);
        }

        [Test]
        public void persistor_is_a_singleton()
        {
            var p1 = theContainer.GetInstance<IPersistor>().ShouldBeOfType<InMemoryPersistor>();

            p1.ShouldBeTheSameAs(theContainer.GetInstance<IPersistor>());
        }

        [Test]
        public void transaction_is_registered()
        {
            theContainer.GetInstance<ITransaction>().ShouldBeOfType<InMemoryTransaction>();
        }

        [Test]
        public void entity_registry_is_registered_and_can_be_built()
        {
            theContainer.GetInstance<IEntityRepository>().ShouldBeOfType<EntityRepository>();
        }

        [Test]
        public void persistence_reset_is_registered()
        {
            theContainer.GetInstance<IPersistenceReset>().ShouldBeOfType<InMemoryPersistenceReset>();
        }

        [Test]
        public void storage_registry_is_registered()
        {
            theContainer.GetInstance<IStorageFactory>().ShouldBeOfType<StorageFactory>();
        }

        [Test]
        public void can_build_out_complete_reset()
        {
            theContainer.GetInstance<ICompleteReset>().ShouldNotBeNull();
        }


        public class TestRegistry : Registry
        {
            public TestRegistry()
            {
                IncludeRegistry<InMemoryPersistenceRegistry>();
            }
        }
    }
}