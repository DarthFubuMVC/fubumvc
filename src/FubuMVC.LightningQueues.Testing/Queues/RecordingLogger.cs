using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Logging;

namespace FubuMVC.LightningQueues.Testing.Queues
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

        public void Debug(string message, params object[] parameters)
        {
            _debug.Add(message.ToFormat(parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            _info.Add(message.ToFormat(parameters));
        }

        public void Error(string message, Exception exception)
        {
            _outputHook(message + exception);
            _error.Add(message);
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Debug(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void Info(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void InfoMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage<T>(Func<T> message) where T : class, LogTopic
        {
            throw new NotImplementedException();
        }

        public void InfoMessage<T>(Func<T> message) where T : class, LogTopic
        {
            throw new NotImplementedException();
        }
    }
}