using FubuCore.Dates;
using FubuCore.Logging;
using FubuPersistence.MultiTenancy;
using FubuPersistence.Reset;
using FubuPersistence.Storage;
using StructureMap.Configuration.DSL;

namespace FubuPersistence.InMemory
{
    public class PersistenceRegistry : Registry
    {
        public PersistenceRegistry()
        {
            // This acts as "SetServiceIfNone"
            For<ISystemTime>().UseIfNone(() => SystemTime.Default());
            For<IInitialState>().Add<NulloInitialState>();
            For<ITenantContext>().Add<NulloTenantContext>();

            // It's important that these are in this order
            For<IEntityStoragePolicy>().Add<ByTenantStoragePolicy>();
            For<IEntityStoragePolicy>().Add<SoftDeletedStoragePolicy>();

            For<IEntityRepository>().Use<EntityRepository>();
            For<IStorageFactory>().Use<StorageFactory>();



            For<ICompleteReset>().Use<CompleteReset>();
            For<ILogger>().Use<Logger>();
        }
    }

    public class InMemoryPersistenceRegistry : Registry
    {
        public InMemoryPersistenceRegistry()
        {
            IncludeRegistry<PersistenceRegistry>();

            For<ITransaction>().Use<InMemoryTransaction>();
            ForSingletonOf<IPersistor>().Use<InMemoryPersistor>();

            For<IPersistenceReset>().Use<InMemoryPersistenceReset>();

            For<IUnitOfWork>().Use<InMemoryUnitOfWork>();


        }
    }
}