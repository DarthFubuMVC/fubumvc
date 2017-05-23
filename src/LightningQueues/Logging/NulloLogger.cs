using System;

namespace LightningQueues.Logging
{
    public class NulloLogger : ILogger
    {
        public void Debug(string message)
        {
        }

        public void DebugFormat(string message, params object[] args)
        {
        }

        public void DebugFormat(string message, object arg1, object arg2)
        {
        }

        public void DebugFormat(string message, object arg1)
        {
        }

        public void Info(string message)
        {
            
        }

        public void InfoFormat(string message, params object[] args)
        {
        }

        public void InfoFormat(string message, object arg1, object arg2)
        {
        }

        public void InfoFormat(string message, object arg1)
        {
        }

        public void Error(string message, Exception exception)
        {
        }
    }
}