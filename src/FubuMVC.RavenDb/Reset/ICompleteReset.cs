namespace FubuPersistence.Reset
{
    public interface ICompleteReset
    {
        void ResetState();
        void CommitChanges();
    }
}