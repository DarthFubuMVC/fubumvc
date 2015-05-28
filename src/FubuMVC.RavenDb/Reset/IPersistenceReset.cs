namespace FubuPersistence.Reset
{
    public interface IPersistenceReset
    {
        void ClearPersistedState();
        void CommitAllChanges();
    }
}