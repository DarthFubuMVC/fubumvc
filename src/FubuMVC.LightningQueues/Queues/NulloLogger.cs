using System;
using FubuCore.Logging;

namespace FubuMVC.LightningQueues.Queues
{
    public class NulloLogger : ILogger
    {
        public void Debug(string message, params object[] parameters)
        {
            
        }

        public void Info(string message, params object[] parameters)
        {
        }

        public void Error(string message, Exception ex)
        {
        }

        public void Error(object correlationId, string message, Exception ex)
        {
        }

        public void Debug(Func<string> message)
        {
        }

        public void Info(Func<string> message)
        {
        }

        public void DebugMessage(LogTopic message)
        {
        }

        public void InfoMessage(LogTopic message)
        {
        }

        public void DebugMessage<T>(Func<T> message) where T : class, LogTopic
        {
        }

        public void InfoMessage<T>(Func<T> message) where T : class, LogTopic
        {
        }
    }
}