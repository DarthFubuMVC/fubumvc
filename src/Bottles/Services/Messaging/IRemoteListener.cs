namespace Bottles.Services.Messaging
{
    public interface IRemoteListener
    {
        void Send(string json);
    }
}