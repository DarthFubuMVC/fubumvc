using System.Collections.Generic;

namespace FubuMVC.Core.Services.Messaging
{
    public interface IMessagingHub
    {
        IEnumerable<object> Listeners { get; }
        void AddListener(object listener);
        void RemoveListener(object listener);
        void Send<T>(T message);
        void SendJson(string json);
    }
}