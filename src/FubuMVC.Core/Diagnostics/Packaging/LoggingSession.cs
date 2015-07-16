using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public class LoggingSession
    {
        private readonly IPerfTimer _timer;

        public LoggingSession(IPerfTimer timer)
        {
            _timer = timer;

            _logs = new Cache<object, ActivationLog>(o => new ActivationLog(_timer)
            {
                Description = o.ToString()
            });
        }

        public LoggingSession() : this(new PerfTimer())
        {
        }

        private readonly Cache<object, ActivationLog> _logs;

        public void LogObject(object target, string provenance)
        {
            _logs[target].Provenance = provenance;
        }

        public IActivationLog LogFor(object target)
        {
            return _logs[target];
        }

        public void LogExecution(object target, Action continuation)
        {
            _logs[target].Execute(continuation);
        }

        public void EachLog(Action<object, ActivationLog> action)
        {
            _logs.Each(action);
        }

        public bool HasErrors()
        {
            return _logs.GetAll().Any(x => !x.Success);
        }

        public IEnumerable<LogSubject> LogsForSubjectType<T>()
        {
            foreach (var pair in _logs.ToDictionary())
            {
                if (pair.Key.GetType().CanBeCastTo<T>())
                {
                    yield return new LogSubject
                    {
                        Subject = pair.Key,
                        Log = pair.Value
                    };
                }
            }
        }
    }

    public class LogSubject
    {
        public object Subject { get; set; }
        public ActivationLog Log { get; set; }
    }
}