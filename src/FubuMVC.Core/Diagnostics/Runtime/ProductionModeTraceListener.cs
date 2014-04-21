using System;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class ProductionModeTraceListener : ILogListener
    {
        private readonly IRequestTrace _trace;

        public ProductionModeTraceListener(IRequestTrace trace)
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