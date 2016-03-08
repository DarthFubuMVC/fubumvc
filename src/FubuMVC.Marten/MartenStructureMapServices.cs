using Marten;
using StructureMap;

namespace FubuMVC.Marten
{
    public class MartenStructureMapServices : Registry
    {
        public MartenStructureMapServices()
        {
            ForSingletonOf<IDocumentStore>().Use<DocumentStore>();
            For<IQuerySession>().Use(c => c.GetInstance<IDocumentStore>().QuerySession());
            For<IDocumentSession>().Use(c => c.GetInstance<ISessionBoundary>().Session());
            For<ISessionBoundary>().Use<SessionBoundary>();
            For<ITransaction>().Use<MartenTransaction>();


            ForSingletonOf<IPersistenceReset>().Use<MartenPersistenceReset>();
            ForSingletonOf<ICompleteReset>().Use<CompleteReset>();
            For<IInitialState>().UseIfNone<NulloInitialState>();
        }

    }
}