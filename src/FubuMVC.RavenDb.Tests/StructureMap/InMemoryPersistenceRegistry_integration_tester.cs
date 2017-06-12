using System;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.Reset;
using FubuMVC.RavenDb.Storage;
using Shouldly;
using StructureMap;
using Xunit;

namespace FubuMVC.RavenDb.Tests.StructureMap
{
    public class InMemoryPersistenceRegistry_integration_tester
    {
        private IContainer theContainer;

        public InMemoryPersistenceRegistry_integration_tester()
        {
            theContainer = new Container(x => x.AddRegistry<TestRegistry>());
        }

        private IEntityRepository theRepository
        {
            get { return theContainer.GetInstance<IEntityRepository>(); }
        }


        [Fact]
        public void can_persist_and_retrieve_an_entities()
        {
            var entity = new FakeEntity {Id = Guid.NewGuid()};

            theRepository.Update(entity);

            theRepository.Find<FakeEntity>(entity.Id).ShouldBeTheSameAs(entity);
        }

        [Fact]
        public void persistor_is_a_singleton()
        {
            var p1 = theContainer.GetInstance<IPersistor>().ShouldBeOfType<InMemoryPersistor>();

            p1.ShouldBeTheSameAs(theContainer.GetInstance<IPersistor>());
        }

        [Fact]
        public void transaction_is_registered()
        {
            theContainer.GetInstance<ITransaction>().ShouldBeOfType<InMemoryTransaction>();
        }

        [Fact]
        public void entity_registry_is_registered_and_can_be_built()
        {
            theContainer.GetInstance<IEntityRepository>().ShouldBeOfType<EntityRepository>();
        }

        [Fact]
        public void persistence_reset_is_registered()
        {
            theContainer.GetInstance<IPersistenceReset>().ShouldBeOfType<InMemoryPersistenceReset>();
        }

        [Fact]
        public void storage_registry_is_registered()
        {
            theContainer.GetInstance<IStorageFactory>().ShouldBeOfType<StorageFactory>();
        }

        [Fact]
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
