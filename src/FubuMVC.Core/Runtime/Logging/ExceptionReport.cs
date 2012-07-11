using System;

namespace FubuMVC.Core.Runtime.Logging
{
    public class ExceptionReport : LogRecord
    {
        public ExceptionReport(string message, Exception exception)
        {
            Message = message;
            ExceptionText = exception.ToString();
        }

        public ExceptionReport()
        {
        }

        public string Message { get; set; }
        public string ExceptionText { get; set; }
        public object CorrelationId { get; set; }
    }
}