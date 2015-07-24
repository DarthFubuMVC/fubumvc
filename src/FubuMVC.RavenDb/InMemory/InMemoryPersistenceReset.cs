using FubuCore;
using FubuMVC.RavenDb.Reset;

namespace FubuMVC.RavenDb.InMemory
{
    public class InMemoryPersistenceReset : IPersistenceReset
    {
        private readonly InMemoryPersistor _persistor;

        public InMemoryPersistenceReset(IPersistor persistor)
        {
            _persistor = persistor.As<InMemoryPersistor>();
        }

        public void ClearPersistedState()
        {
            _persistor.WipeAndReplace(new IEntity[0]);
        }

        public void CommitAllChanges()
        {
            // no-op
        }
    }
}