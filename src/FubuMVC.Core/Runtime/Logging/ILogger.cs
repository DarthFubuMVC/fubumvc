using System;

namespace FubuMVC.Core.Runtime.Logging
{
    public interface ILogger
    {
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Error(string message, Exception ex);
        void Error(object correlationId, string message, Exception ex);

        void Debug(Func<string> message);
        void Info(Func<string> message);

        void DebugMessage(object message);
        void InfoMessage(object message);

        void DebugMessage<T>(Func<T> message);
        void InfoMessage<T>(Func<T> message);
    }
}