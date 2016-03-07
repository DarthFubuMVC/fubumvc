using Marten;

namespace FubuMVC.Marten
{
    public class MartenPersistenceReset : IPersistenceReset
    {
        private readonly IDocumentStore _store;

        public MartenPersistenceReset(IDocumentStore store)
        {
            _store = store;
        }

        public void ClearPersistedState()
        {
            _store.Advanced.Clean.DeleteAllDocuments();
        }

        public void CommitAllChanges()
        {
            // No-op. Don't rememb er what this was for, shamefully.
        }
    }
}