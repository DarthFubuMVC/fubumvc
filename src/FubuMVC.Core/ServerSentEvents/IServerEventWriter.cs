namespace FubuMVC.Core.ServerSentEvents
{
    public interface IServerEventWriter
    {
        bool WriteData(object data, string id = null, string @event = null, int? retry = null);
        bool Write(IServerEvent @event);
    }
}