namespace FubuMVC.Core.Runtime.Files
{
    public interface IChangeSetHandler
    {
        void Handle(ChangeSet changes);
    }
}