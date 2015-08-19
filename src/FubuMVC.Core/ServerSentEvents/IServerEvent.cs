namespace FubuMVC.Core.ServerSentEvents
{
    public interface IServerEvent
    {
        string Id { get; }
        string Event { get; }
        int? Retry { get; }
        object Data { get; }
    }
}