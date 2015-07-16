namespace FubuMVC.Core.Services.Messaging
{
    public interface IRemoteListener
    {
        void Send(string json);
    }
}