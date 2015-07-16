namespace FubuMVC.Core.Services.Messaging
{
    public interface IListener
    {
        void Receive<T>(T message);
    }

    public interface IListener<T>
    {
        void Receive(T message);
    }
}