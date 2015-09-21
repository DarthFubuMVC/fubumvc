using System;
using FubuCore.Logging;
using ExceptionReport = FubuCore.Logging.ExceptionReport;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class ChainExecutionListener : ILogListener
    {
        private readonly IChainExecutionLog _trace;

        public ChainExecutionListener(IChainExecutionLog trace)
        {
            _trace = trace;
        }

        public bool ListensFor(Type type)
        {
            return true;
        }

        public void DebugMessage(object message)
        {
            _trace.Log(message);
        }

        public void InfoMessage(object message)
        {
            _trace.Log(message);
        }

        public void Debug(string message)
        {
            _trace.Log(new StringMessage(message));
        }

        public void Info(string message)
        {
            _trace.Log(new StringMessage(message));
        }

        public void Error(string message, Exception ex)
        {
            _trace.LogException(ex);
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _trace.LogException(ex);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }
    }
}