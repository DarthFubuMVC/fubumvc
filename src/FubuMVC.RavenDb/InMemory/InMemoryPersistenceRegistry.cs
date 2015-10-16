using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.RavenDb.MultiTenancy;
using FubuMVC.RavenDb.Reset;
using FubuMVC.RavenDb.Storage;
using StructureMap;

namespace FubuMVC.RavenDb.InMemory
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


            // Needs to be resolved from the parent
            ForSingletonOf<ICompleteReset>().Use<CompleteReset>();
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