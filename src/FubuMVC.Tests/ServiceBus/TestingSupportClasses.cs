using System;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus.Events;

namespace FubuMVC.Tests.ServiceBus
{

    public class StubListener<T> : IListener<T>
    {
        public T LastMessage { get; set; }

        #region IListener<T> Members

        public void Handle(T message)
        {
            LastMessage = message;
        }

        #endregion
    }

    public class ErrorCausingHandler : IListener<Message1>
    {
        public void Handle(Message1 message)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ExpiringListener : IExpiringListener
    {
        public bool IsExpired { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class TaskHandler
    {
        public Task AsyncHandle(Message1 message)
        {
            return null;
        }

        public Task<Message2> AsyncReturn(Message1 message)
        {
            return null;
        }
    }

    public class Message1
    {
    }

    public class Message2
    {
    }

    public class Message3
    {
    }

    public class Message4
    {
    }

    public class Message5
    {
    }

    public class Message6
    {
    }

    public interface IMessageHandler1
    {
        void HandleMessage(Message1 message);
    }

    public interface IMessageHandler2
    {
        void HandleMessage(Message2 message);
    }


    public class StubMessage1Handler : IListener<Message1>
    {
        public Message1 Message { get; set; }

        #region IListener<Message1> Members

        public void Handle(Message1 message)
        {
            Message = message;
        }

        #endregion
    }
}