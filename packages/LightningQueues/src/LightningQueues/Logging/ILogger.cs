using System;
using System.Collections.Generic;
using LightningQueues.Model;
using LightningQueues.Protocol;

namespace LightningQueues.Logging
{
    public interface ILogger
    {
        void Debug(string message, params object[] args);
        void Debug(Func<string> message);
        void Error(string message, Exception exception);
        void Info(string message, params object[] args);
        void Info(string message, Exception exception, params object[] args);

        void FailedToSend(Endpoint destination, string reason, Exception exception = null);
        void QueuedForReceive(Message message);
        void QueuedForSend(Message message, Endpoint destination);
        void MessageReceived(Message message);
        void MessagesSent(IList<Message> messages, Endpoint destination);
    }
}