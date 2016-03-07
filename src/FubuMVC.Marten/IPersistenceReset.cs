namespace FubuMVC.Marten
{
    public interface IPersistenceReset
    {
        void ClearPersistedState();
        void CommitAllChanges();
    }
}