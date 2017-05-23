using System;
using System.Collections.Generic;
using LightningQueues.Logging;

namespace LightningQueues.Tests
{
    public class RecordingLogger : ILogger
    {
        private readonly Action<string> _outputHook;
        readonly IList<string> _debug = new List<string>();
        readonly IList<string> _error = new List<string>();
        readonly IList<string> _info = new List<string>();

        public RecordingLogger() : this(x => { })
        {
        }

        public RecordingLogger(Action<string> outputHook)
        {
            _outputHook = outputHook;
        }



        public IEnumerable<string> DebugMessages => _debug;
        public IEnumerable<string> InfoMessages => _info;

        public IEnumerable<string> ErrorMessages => _error;

        public void Debug(string message)
        {
            _outputHook(message);
            _debug.Add(message);
        }

        public void DebugFormat(string message, params object[] args)
        {
            Debug(string.Format(message, args));
        }

        public void DebugFormat(string message, object arg1, object arg2)
        {
            Debug(string.Format(message, arg1, arg2));
        }

        public void DebugFormat(string message, object arg1)
        {
            Debug(string.Format(message, arg1));
        }

        public void Info(string message)
        {
            _info.Add(message);
        }

        public void InfoFormat(string message, params object[] args)
        {
            _info.Add(string.Format(message, args));
        }

        public void InfoFormat(string message, object arg1, object arg2)
        {
            _info.Add(string.Format(message, arg1, arg2));
        }

        public void InfoFormat(string message, object arg1)
        {
            _info.Add(string.Format(message, arg1));
        }

        public void Error(string message, Exception exception)
        {
            _outputHook(message + exception);
            _error.Add(message);
        }
    }
}