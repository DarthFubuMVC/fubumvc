using System;
using FubuCore.Logging;

namespace FubuMVC.LightningQueues
{
    public class FubuLoggingAdapter : global::LightningQueues.Logging.ILogger
    {
        private readonly ILogger _logger;

        public FubuLoggingAdapter(ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void DebugFormat(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void DebugFormat(string message, object arg1, object arg2)
        {
            _logger.Debug(string.Format(message, arg1, arg2));
        }

        public void DebugFormat(string message, object arg1)
        {
            _logger.Debug(string.Format(message, arg1));
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void InfoFormat(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void InfoFormat(string message, object arg1, object arg2)
        {
            _logger.Info(message, arg1, arg2);
        }

        public void InfoFormat(string message, object arg1)
        {
            _logger.Info(message, arg1);
        }
    }
}
