using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Runtime.Logging
{
    public class RecordingLogger : ILogger
    {
        private readonly IList<LogRecord> _debug = new List<LogRecord>();
        private readonly IList<LogRecord> _info = new List<LogRecord>();
        private readonly IList<ExceptionReport> _errors = new List<ExceptionReport>();

        public IEnumerable<LogRecord> DebugMessages
        {
            get { return _debug; }
        }

        public IEnumerable<LogRecord> InfoMessages
        {
            get { return _info; }
        }

        public IEnumerable<LogRecord> ErrorMessages
        {
            get { return _errors; }
        }

        public void Debug(string message, params object[] parameters)
        {
            _debug.Add(new StringMessage(message, parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            _info.Add(new StringMessage(message, parameters));
        }

        public void Error(string message, Exception ex)
        {
            _errors.Add(new ExceptionReport(message, ex));
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _errors.Add(new ExceptionReport{
                Message = message,
                ExceptionText = ex.ToString(),
                CorrelationId = correlationId
            });
        }

        public void Debug(Func<string> message)
        {
            Debug(message());
        }

        public void Info(Func<string> message)
        {
            Info(message());
        }

        public void DebugMessage(LogRecord message)
        {
            _debug.Add(message);
        }

        public void InfoMessage(LogRecord message)
        {
            _info.Add(message);
        }

        public void DebugMessage<T>(Func<T> message) where T : LogRecord
        {
            _debug.Add(message());
        }

        public void InfoMessage<T>(Func<T> message) where T : LogRecord
        {
            _info.Add(message());
        }
    }
}