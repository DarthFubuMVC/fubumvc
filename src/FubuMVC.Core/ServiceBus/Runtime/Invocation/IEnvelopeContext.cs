using System;
using System.Collections.Generic;
using FubuCore.Dates;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IEnvelopeContext : IDisposable
    {
        void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages);
        void SendFailureAcknowledgement(Envelope original, string message);
        ISystemTime SystemTime { get; }
        void InfoMessage<T>(Func<T> func) where T : class, LogTopic;
        void DebugMessage<T>(Func<T> func) where T : class, LogTopic;
        void InfoMessage<T>(T message) where T : LogTopic;

        void Error(string correlationId, string message, Exception exception);
        void Retry(Envelope envelope);
    }

}