using FubuMVC.Core.Runtime;
using FubuMVC.RavenDb.RavenDb;
using Raven.Client;
using StructureMap;

namespace FubuMVC.RavenDb
{
    public static class DatabaseReplacementExtensions
    {
        /// <summary>
        /// Change the default RavenDb datastore to the configured settings
        /// </summary>
        /// <param name="container"></param>
        /// <param name="settings"></param>
        public static void ReplaceDatastore(this IContainer container, RavenDbSettings settings)
        {
            container.Model.For<IDocumentStore>().Default.EjectObject();
            container.Model.For<RavenDbSettings>().EjectAndRemoveAll();

            container.Inject(settings);
        }

        /// <summary>
        /// Change the default RavenDb datastore to the configured settings
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="settings"></param>
        public static void ReplaceDatastore(this IServiceFactory factory, RavenDbSettings settings)
        {
            factory.Get<IContainer>().ReplaceDatastore(settings);
        }

        /// <summary>
        /// Replace the current datastore with a new in memory datastore
        /// </summary>
        /// <param name="container"></param>
        public static void UseInMemoryDatastore(this IContainer container)
        {
            container.ReplaceDatastore(RavenDbSettings.InMemory());
        }

        /// <summary>
        /// Replace the current datastore with a new in memory datastore
        /// </summary>
        /// <param name="factory"></param>
        public static void UseInMemoryDatastore(this IServiceFactory factory)
        {
            factory.Get<IContainer>().UseInMemoryDatastore();
        }
    }
}