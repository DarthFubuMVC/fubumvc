namespace FubuMVC.Core.View.Model.Sharing
{
    public interface ISharingRegistration
    {
        void Global(string global);
        void Dependency(string dependent, string dependency);
    }
}