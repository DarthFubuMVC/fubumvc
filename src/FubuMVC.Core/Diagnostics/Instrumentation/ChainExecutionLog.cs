using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class ChainExecutionLog : IRequestLog, ISubject, IChainExecutionLog
    {
        private readonly Dictionary<string, object> _request;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly Activity _activity;
        private readonly Stack<Activity> _activityStack = new Stack<Activity>();
        private readonly IList<Exception> _exceptions = new List<Exception>();
        private readonly IList<RequestStep> _steps = new List<RequestStep>(); 

        public ChainExecutionLog()
        {
            _request = new Dictionary<string, object>();
            _stopwatch.Start();
            Id = Guid.NewGuid();

            _activity = new Activity(this, 0);
            _activityStack.Push(_activity);
        }

        public IEnumerable<RequestStep> Steps
        {
            get { return _steps; }
        }

        public IDictionary<string, object> ToDictionary()
        {
            var dict = ToHeaderDictionary();

            var steps = Steps.Select(x =>
            {
                return new Dictionary<string, object>
                {
                    {"activity", x.Activity.Subject.Id},
                    {"log", Description.For(x.Log).ToDictionary()},
                    {"time", x.RequestTime}
                };
            }).ToArray();
            dict.Add("steps", steps);


            var activities = AllActivities().Select(x =>
            {
                return new Dictionary<string, object>
                {
                    {"title", x.Subject.Title()},
                    {"start", x.Start},
                    {"end", x.End},
                    {"duration", x.Duration},
                    {"inner_time", x.InnerTime},
                    {"id", x.Subject.Id.ToString()}
                };
            }).ToArray();

            dict.Add("activities", activities);


            return dict;
        }

        public Dictionary<string, object> ToHeaderDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                {"request", _request},
                {"time", Time.ToLocalTime().ToShortTimeString()},
                {"execution_time", ExecutionTime},
                {"title", Title()},
                {"id", Id.ToString()},
                {"exceptions", HadException}
            };

            if (RootChain != null) dict.Add("chain", RootChain.Key);
            return dict;
        }

        public IEnumerable<Activity> AllActivities()
        {
            return Activity.AllActivities().Distinct();
        }

        public readonly DateTime Time = DateTime.UtcNow;

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

        public BehaviorChain RootChain { get; set; }

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
            Log(new ExceptionReport(ex));
        }

        public void MarkFinished()
        {
            _exceptions.Clear();
            _stopwatch.Stop();
            _activity.MarkEnd(requestTime());
            ExecutionTime = requestTime();
        }

        public Dictionary<string, object> Request
        {
            get { return _request; }
        }

        public string SessionTag { get; set; }

        public void Log(object log)
        {
            var step = new RequestStep(requestTime(), log) {Activity = _activityStack.Peek()};

            _steps.Add(step);
        }

        // acts like the timer in diagnostics
        public void Trace(string description, Action action)
        {
            var start = requestTime();
            action();
            var finish = requestTime();

            Log(new Trace
            {
                Description = description,
                Duration = finish - start
            });
        }

        public void RecordHeaders(IDictionary<string, object> env)
        {
            env.CopyTo(_request, "owin.RequestHeaders", "owin.RequestMethod", "owin.RequestPath", "owin.RequestPathBase",
                "owin.RequestProtocol", "owin.RequestQueryString", "owin.RequestScheme", "owin.ResponseHeaders",
                "owin.ResponseStatusCode", "owin.ResponseReasonPhrase");
        }

        public void RecordBody(IDictionary<string, object> env)
        {
            // TODO -- will need to get the request body somehow
        }

        public void RecordHeaders(Envelope envelope)
        {
            var headers = envelope.Headers.ToNameValues();
            var dict = new Dictionary<string, object>();
            headers.AllKeys.Each(key =>
            {
                dict.Add(key, headers[key]);
            });

            _request.Add("headers", dict);
        }

        public void RecordBody(Envelope envelope)
        {
            // TODO -- grab the message body if it isn't too big?
        }
    }
}