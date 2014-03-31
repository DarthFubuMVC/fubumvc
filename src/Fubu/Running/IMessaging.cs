namespace Fubu.Running
{
    public interface IMessaging
    {
        void Send<T>(T message);
    }
}