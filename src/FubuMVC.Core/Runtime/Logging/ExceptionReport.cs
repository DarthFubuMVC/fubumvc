using System;
using FubuCore.Logging;

namespace FubuMVC.Core.Runtime.Logging
{
    public class ExceptionReport : LogRecord
    {
        public ExceptionReport(string message, Exception exception)
        {
            Message = message;
            ExceptionType = exception.GetType().Name;
            ExceptionText = exception.ToString();
        }

        public ExceptionReport(Exception exception) : this(exception.Message, exception)
        {
        }

        public ExceptionReport()
        {
        }

        public string ExceptionType { get; set; }

        public string Message { get; set; }
        public string ExceptionText { get; set; }
        public object CorrelationId { get; set; }

        public bool Equals(ExceptionReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Message, Message) && Equals(other.ExceptionText, ExceptionText) &&
                   Equals(other.CorrelationId, CorrelationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExceptionReport)) return false;
            return Equals((ExceptionReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Message != null ? Message.GetHashCode() : 0);
                result = (result*397) ^ (ExceptionText != null ? ExceptionText.GetHashCode() : 0);
                result = (result*397) ^ (CorrelationId != null ? CorrelationId.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("Message: {0}, ExceptionText: {1}, CorrelationId: {2}", Message, ExceptionText,
                                 CorrelationId);
        }
    }
}