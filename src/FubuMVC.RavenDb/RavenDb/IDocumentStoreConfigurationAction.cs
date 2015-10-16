using System;
using FubuMVC.RavenDb.RavenDb.Multiple;
using Raven.Client;
using StructureMap;

namespace FubuMVC.RavenDb.RavenDb
{
    public interface IDocumentStoreConfigurationAction
    {
        void Configure(IDocumentStore documentStore);
    }

    public interface IDocumentStoreConfigurationAction<T> : IDocumentStoreConfigurationAction where T : RavenDbSettings
    {
        
    }

    public class LambdaDocumentStoreConfigurationAction<T> : IDocumentStoreConfigurationAction<T> where T : RavenDbSettings
    {
        private readonly Action<IDocumentStore> _configuration;

        public static IDocumentStoreConfigurationAction<T> For(Action<IDocumentStore> configuration)
        {
            return new LambdaDocumentStoreConfigurationAction<T>(configuration);
        }

        private LambdaDocumentStoreConfigurationAction(Action<IDocumentStore> configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IDocumentStore documentStore)
        {
            _configuration(documentStore);
        }
    }

    public static class StructureMapRegistryExtensions
    {
        public static void RavenDbConfiguration(this Registry registry, Action<IDocumentStore> configuration)
        {
            var action = LambdaDocumentStoreConfigurationAction<RavenDbSettings>.For(configuration);
            registry.For<IDocumentStoreConfigurationAction>()
                    .Add(action);
        }

        public static MultipleDatabaseRegistrationExpression<T> ConnectToRavenDb<T>(this Registry registry, Action<IDocumentStore> configuration = null) where T : RavenDbSettings
        {
            registry.ForSingletonOf<IDocumentStore<T>>().UseInstance(new DocumentStoreInstance<T>());

            if (configuration != null)
            {
                var action = LambdaDocumentStoreConfigurationAction<T>.For(configuration);
                registry.For<IDocumentStoreConfigurationAction<T>>()
                        .Add(action);
            }

            return new MultipleDatabaseRegistrationExpression<T>(registry);
        }

    }


    public class MultipleDatabaseRegistrationExpression<T>  where T : RavenDbSettings
    {
        private readonly Registry _registry;

        public MultipleDatabaseRegistrationExpression(Registry registry)
        {
            _registry = registry;
        }

        public void Using<TInterface, TClass>()
            where TInterface : IDocumentSession<T> 
            where TClass : DocumentSession<T>, TInterface
        {
            _registry.For<TInterface>().Use<TClass>();
        }

    }
}