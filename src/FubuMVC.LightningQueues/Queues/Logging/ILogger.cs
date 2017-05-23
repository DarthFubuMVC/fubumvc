using System;

namespace FubuMVC.LightningQueues.Queues.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void DebugFormat(string message, params object[] args);
        void DebugFormat(string message, object arg1, object arg2);
        void DebugFormat(string message, object arg1);


        void Info(string message);
        void InfoFormat(string message, params object[] args);
        void InfoFormat(string message, object arg1, object arg2);
        void InfoFormat(string message, object arg1);

        void Error(string message, Exception exception);
    }
}