using System;
using System.Diagnostics;
using FubuCore;
using FubuMVC.Core.Services;
using Topshelf.HostConfigurators;
using Topshelf.Logging;

namespace JasperService
{
    public static class HostConfiguratorLogExtensions
    {
        public static void UseEventLog(this HostConfigurator configurator, JasperServiceConfiguration settings)
        {
            HostLogger.UseLogger(new EventLogFactoryConfigurator(settings));
        }

        public class EventLogFactoryConfigurator : HostLoggerConfigurator
        {
            private readonly JasperServiceConfiguration _settings;

            public EventLogFactoryConfigurator(JasperServiceConfiguration settings)
            {
                _settings = settings;
            }

            public LogWriterFactory CreateLogWriterFactory()
            {
                return new EventLogFactory(_settings);
            }
        }
    }

    public class EventLogFactory : LogWriterFactory
    {
        private readonly JasperServiceConfiguration _settings;

        public EventLogFactory(JasperServiceConfiguration settings)
        {
            _settings = settings;
            if (!EventLog.SourceExists(_settings.Name))
                EventLog.CreateEventSource(_settings.Name, "Application");
        }

        public LogWriter Get(string name)
        {
            return new EventLogWriter(_settings, name);
        }

        public void Shutdown()
        {
            //no-op
        }
    }

    public class EventLogWriter : LogWriter
    {
        private readonly JasperServiceConfiguration _settings;
        private readonly string _name;

        public EventLogWriter(JasperServiceConfiguration settings, string name)
        {
            _settings = settings;
            _name = name;
        }

        public void Log(LoggingLevel level, object obj)
        {
            // no-op
        }

        public void Log(LoggingLevel level, object obj, Exception exception)
        {
            // no-op
        }

        public void Log(LoggingLevel level, LogWriterOutputProvider messageProvider)
        {
            // no-op
        }

        public void LogFormat(LoggingLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            // no-op
        }

        public void LogFormat(LoggingLevel level, string format, params object[] args)
        {
            // no-op
        }

        public void Debug(object obj)
        {
            // no-op
        }

        public void Debug(object obj, Exception exception)
        {
            // no-op
        }

        public void Debug(LogWriterOutputProvider messageProvider)
        {
            // no-op
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            // no-op
        }

        public void DebugFormat(string format, params object[] args)
        {
            // no-op
        }

        public void Info(object obj)
        {
            // no-op
        }

        public void Info(object obj, Exception exception)
        {
            // no-op
        }

        public void Info(LogWriterOutputProvider messageProvider)
        {
            // no-op
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            // no-op
        }

        public void InfoFormat(string format, params object[] args)
        {
            // no-op
        }

        public void Warn(object obj)
        {
            // no-op
        }

        public void Warn(object obj, Exception exception)
        {
            // no-op
        }

        public void Warn(LogWriterOutputProvider messageProvider)
        {
            // no-op
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            // no-op
        }

        public void WarnFormat(string format, params object[] args)
        {
            // no-op
        }

        public void Error(object obj)
        {
            Log(obj, EventLogEntryType.Error);
        }

        public void Error(object obj, Exception exception)
        {
            Log("{0}{1}{2}".ToFormat(obj, Environment.NewLine, exception), EventLogEntryType.Error);
        }

        public void Error(LogWriterOutputProvider messageProvider)
        {
            Error(messageProvider());
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Error(string.Format(formatProvider, format, args));
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Error(format.ToFormat(args));
        }

        public void Fatal(object obj)
        {
            Error(obj);
        }

        public void Fatal(object obj, Exception exception)
        {
            Error(obj, exception);
        }

        public void Fatal(LogWriterOutputProvider messageProvider)
        {
            Error(messageProvider());
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            ErrorFormat(formatProvider, format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            ErrorFormat(format, args);
        }

        public bool IsDebugEnabled { get { return false; } }
        public bool IsInfoEnabled { get { return false; } }
        public bool IsWarnEnabled { get { return false; } }
        public bool IsErrorEnabled { get { return true; } }
        public bool IsFatalEnabled { get { return true; } }

        private void Log(object obj, EventLogEntryType type)
        {
            // Event Log's max message size is 31839 characters
            string message = "[{0}]: {1}".ToFormat(_name, obj);
            message = message.Substring(0, Math.Min(message.Length, 31839));
            EventLog.WriteEntry(_settings.Name, message, type);
        }
    }
}
