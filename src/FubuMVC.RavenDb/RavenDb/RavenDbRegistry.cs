using FubuMVC.Core.Registration;
using FubuMVC.RavenDb.InMemory;
using FubuMVC.RavenDb.RavenDb.Multiple;
using FubuMVC.RavenDb.Reset;
using Raven.Client;

namespace FubuMVC.RavenDb.RavenDb
{
    public class RavenDbRegistry : ServiceRegistry
    {
        public RavenDbRegistry()
        {
            IncludeRegistry<PersistenceRegistry>();

            Scan(x =>
            {
                x.AssemblyContainingType<IDocumentStoreConfigurationAction>();
                x.AddAllTypesOf<IDocumentStoreConfigurationAction>();
            });

            
            ForSingletonOf<IDocumentStore>().Use(c => c.GetInstance<DocumentStoreBuilder>().Build());

            For<IDocumentSession>().Use(c => c.GetInstance<ISessionBoundary>().Session());

            For<ISessionBoundary>().Use<SessionBoundary>();

            For<IPersistor>().Use<RavenPersistor>();

            For<ITransaction>().Use<RavenTransaction>();

            For<IUnitOfWork>().Use<RavenUnitOfWork>();

            ForSingletonOf<IPersistenceReset>().Use<RavenPersistenceReset>();

            For(typeof(IDocumentSession<>)).Use(typeof(DocumentSession<>));
        }
    }
}