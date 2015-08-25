using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class ChainExecutionLog :IRequestLog, ISubject, IChainExecutionLog
    {
        private readonly IDictionary<string, object> _request;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly Activity _activity;
        private readonly Stack<Activity> _activityStack = new Stack<Activity>(); 
        private readonly IList<Exception> _exceptions = new List<Exception>(); 

        public ChainExecutionLog()
        {
            _request = new Dictionary<string, object>();
            _stopwatch.Start();
            Id = Guid.NewGuid();

            _activity = new Activity(this, 0);
            _activityStack.Push(_activity);
        }

        public Activity Activity
        {
            get { return _activity; }
        }

        protected virtual double requestTime()
        {
            return _stopwatch.ElapsedMilliseconds;
        }

        private Activity current
        {
            get { return _activityStack.Any() ? _activityStack.Peek() : _activity; }
        }

        public void StartSubject(ISubject subject)
        {
            var activity = new Activity(subject, requestTime());
            current.Nested.Add(activity);

            _activityStack.Push(activity);
        }

        public void FinishSubject()
        {
            if (_activityStack.Any())
            {
                current.MarkEnd(requestTime());
                _activityStack.Pop();
            }
        }

        public BehaviorChain RootChain { get;set; }

        public string Title()
        {
            return RootChain == null ? "Unknown" : RootChain.Title();
        }

        public Guid Id { get; private set; }


        public double ExecutionTime { get; private set; }

        public bool HadException { get; private set; }

        public void LogException(Exception ex)
        {
            if (_exceptions.Contains(ex)) return;

            _exceptions.Add(ex);
            HadException = true;
            AddLog(new ExceptionReport(ex));
        }

        public void MarkFinished(Action<IDictionary<string, object>> writeResponse)
        {
            _exceptions.Clear();
            _stopwatch.Stop();
            ExecutionTime = requestTime();
            writeResponse(_request);
        }

        public IDictionary<string, object> Request
        {
            get { return _request; }
        }

        public void AddLog(object log)
        {
            current.AppendLog(requestTime(), log);
        }

        // acts like the timer in diagnostics
        public void Trace(string description, Action action)
        {
            var start = requestTime();
            action();
            var finish = requestTime();

            AddLog(new Trace
            {
                Description = description,
                Duration = finish - start
            });
        }

        public void RecordHeaders(IDictionary<string, object> env)
        {
            throw new NotImplementedException();
        }

        public void RecordBody(IDictionary<string, object> env)
        {
            throw new NotImplementedException();
        }
    }
}