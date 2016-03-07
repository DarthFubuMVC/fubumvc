namespace FubuMVC.Marten
{
    public interface ICompleteReset
    {
        void ResetState();
        void CommitChanges();
    }
}