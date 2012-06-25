using System;

namespace FubuMVC.Core.Runtime.Logging
{
    public interface ILogListener
    {
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }

        bool ListensFor(Type type);

        void DebugMessage(object message);
        void InfoMessage(object message);
        void Debug(string message);
        void Info(string message);

        void Error(string message, Exception ex);
        void Error(object correlationId, string message, Exception ex);
    }
}