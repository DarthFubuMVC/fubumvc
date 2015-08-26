using System;
using FubuCore.Logging;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class ProductionModeTraceListener : ILogListener
    {
        private readonly IChainExecutionLog _trace;

        public ProductionModeTraceListener(IChainExecutionLog trace)
        {
            _trace = trace;
        }

        public void Error(string message, Exception ex)
        {
            _trace.Log(new ExceptionReport(message, ex));
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _trace.Log(new ExceptionReport(message, ex)
            {
                CorrelationId = correlationId
            });
        }

        public bool ListensFor(Type type)
        {
            return false;
        }

        public void DebugMessage(object message)
        {
        }

        public void InfoMessage(object message)
        {
        }

        public void Debug(string message)
        {
        }

        public void Info(string message)
        {
        }

        public bool IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return false;
            }
        }
    }
}