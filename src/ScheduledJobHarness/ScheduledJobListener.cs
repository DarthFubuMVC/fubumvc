using System;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace ScheduledJobHarness
{
    public class ScheduledJobListener : ILogListener
    {
        public bool ListensFor(Type type)
        {
            return type.Namespace == typeof (ScheduledJobRecord).Namespace;
        }

        public void DebugMessage(object message)
        {
        }

        public void InfoMessage(object message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Error(string message, Exception ex)
        {
        }

        public void Error(object correlationId, string message, Exception ex)
        {
        }

        public bool IsDebugEnabled { get { return true; } }
        public bool IsInfoEnabled { get { return true; } }
    }
}