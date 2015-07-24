namespace FubuMVC.RavenDb.Reset
{
    public interface ICompleteReset
    {
        void ResetState();
        void CommitChanges();
    }
}