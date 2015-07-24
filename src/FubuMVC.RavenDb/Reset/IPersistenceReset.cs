namespace FubuMVC.RavenDb.Reset
{
    public interface IPersistenceReset
    {
        void ClearPersistedState();
        void CommitAllChanges();
    }
}