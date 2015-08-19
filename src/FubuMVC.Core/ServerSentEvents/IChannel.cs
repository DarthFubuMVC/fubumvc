using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServerSentEvents
{
    public interface IChannel
    {
        void Flush();
    }

    public interface IChannel<in T> : IChannel where T : Topic
    {
        Task<IEnumerable<IServerEvent>> FindEvents(T topic);
        void Write(Action<IEventQueue<T>> action);
        bool IsConnected();
    }
}