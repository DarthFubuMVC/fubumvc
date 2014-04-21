using System;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using ExceptionReport = FubuCore.Logging.ExceptionReport;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class RequestTraceListener : ILogListener
    {
        private readonly IRequestTrace _trace;

        public RequestTraceListener(IRequestTrace trace)
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
            _trace.Log(new ExceptionReport(message, ex));
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _trace.Log(new ExceptionReport(message, ex){
                CorrelationId = correlationId
            });
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