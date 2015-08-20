using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface ISubject
    {
        string Title();
        Guid Id { get; }
    }

    public interface IRequestLog
    {
        double ExecutionTime { get; }
        bool HadException { get; }


    }


    /*
     * Subsume ExceptionHandlingObserver
     * 
     */
    public class ChainExecutionLog :IRequestLog, ISubject
    {
        private readonly IDictionary<string, object> _request;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly Activity _activity;
        private readonly Stack<Activity> _activityStack = new Stack<Activity>(); 
        private readonly IList<Exception> _exceptions = new List<Exception>(); 

        private ChainExecutionLog(IDictionary<string, object> request)
        {
            _request = request;
            _stopwatch.Start();
            Id = Guid.NewGuid();

            _activity = new Activity(this, 0);
            _activityStack.Push(_activity);
        }

        public Activity Activity
        {
            get { return _activity; }
        }

        public IEnumerable<RequestStep> AllSteps()
        {
            throw new NotImplementedException();
        } 

        private Activity current
        {
            get { return _activityStack.Any() ? _activityStack.Peek() : _activity; }
        }

        public void StartSubject(ISubject subject)
        {
            var activity = new Activity(subject, _stopwatch.ElapsedMilliseconds);
            current.Nested.Add(activity);

            _activityStack.Push(activity);
        }

        public void FinishSubject()
        {
            if (_activityStack.Any()) _activityStack.Pop();
        }

        public BehaviorChain RootChain { get;set; }

        public string Title()
        {
            return RootChain == null ? "Unknown" : RootChain.Title();
        }

        public Guid Id { get; private set; }

        public static ChainExecutionLog Start(IDictionary<string, object> request)
        {
            return new ChainExecutionLog(request);
        }

        public double ExecutionTime { get; private set; }

        public bool HadException { get; private set; }

        public void LogException(Exception ex)
        {
            _exceptions.Fill(ex);
            HadException = true;
            AddLog(new ExceptionReport(ex));
        }

        public void MarkFinished(Action<IDictionary<string, object>> writeResponse)
        {
            _stopwatch.Stop();
            ExecutionTime = _stopwatch.ElapsedMilliseconds;
            writeResponse(_request);
        }

        public void AddLog(object log)
        {
            current.AppendLog(_stopwatch.ElapsedMilliseconds, log);
        }

        // acts like the timer in diagnostics
        public void Trace(string description, Action action)
        {
            var start = _stopwatch.ElapsedMilliseconds;
            action();
            var finish = _stopwatch.ElapsedMilliseconds;

            AddLog(new Trace
            {
                Description = description,
                Duration = finish - start
            });
        }
    }

    public class Trace
    {
        public string Description { get; set; }
        public double Duration { get; set; }
    }

}