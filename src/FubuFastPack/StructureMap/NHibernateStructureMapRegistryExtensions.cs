using System;
using FubuFastPack.NHibernate;
using FubuFastPack.Persistence;
using NHibernate;
using StructureMap.Configuration.DSL;

namespace FubuFastPack.StructureMap
{
    public static class NHibernateStructureMapRegistryExtensions
    {
        public static void BootstrapNHibernate(this Registry registry, ConfigurationBehavior behavior)
        {
            registry.For<ITransactionProcessor>().Use<TransactionProcessor>();
            registry.For<ISqlRunner>().Use<SqlRunner>();
            registry.For<IRepository>().Use<Repository>();
            registry.For<IEntityFinderCache>().Use<StructureMapEntityFinderCache>();
            registry.For<IEntityFinder>().Use<EntityFinder>();

            if (behavior == ConfigurationBehavior.UsePersistedConfigurationIfItExists)
            {
                registry.ForSingletonOf<IConfigurationSource>().Use<FileCacheConfigurationSource>()
                    .Ctor<IConfigurationSource>().Is<ConfigurationSource>();
            }
            else
            {
                registry.ForSingletonOf<IConfigurationSource>().Use<ConfigurationSource>();
            }

            registry.For<ISchemaWriter>().Use<SchemaWriter>();

            registry.ForSingletonOf<ISessionFactory>().Use(
                c => c.GetInstance<IConfigurationSource>().Configuration().BuildSessionFactory());

            registry.For<ISession>().Use(c => c.GetInstance<ISessionFactory>().OpenSession());
        }

        public static void BootstrapNHibernate<TNHibernateRegistry>(this Registry registry, ConfigurationBehavior behavior) where TNHibernateRegistry : NHibernateRegistry
        {
            registry.For<IConfigurationModifier>().Add<TNHibernateRegistry>();

            registry.BootstrapNHibernate(behavior);
        }

        private static void fetchSessionFromTransactionBoundary(this Registry registry)
        {
            registry.For<ITransactionBoundary>().Use(c => c.GetInstance<INHibernateTransactionBoundary>());
            registry.For<ISession>().Use(c => c.GetInstance<INHibernateTransactionBoundary>().Session);
        }

        public static void UseOnDemandNHibernateTransactionBoundary(this Registry registry)
        {
            registry.For<INHibernateTransactionBoundary>().Use<OnDemandTransactionBoundary>();
            registry.fetchSessionFromTransactionBoundary();
        }

        public static void UseExplicitNHibernateTransactionBoundary(this Registry registry)
        {
            registry.For<INHibernateTransactionBoundary>().Use<ExplicitTransactionBoundary>();
            registry.fetchSessionFromTransactionBoundary();
        }

        public static void RegisterFinder<T>(this Registry registry, Func<IRepository, string, T> finder)
        {
            registry.For<Func<IRepository, string, T>>().Use(finder);
        }
    }
}