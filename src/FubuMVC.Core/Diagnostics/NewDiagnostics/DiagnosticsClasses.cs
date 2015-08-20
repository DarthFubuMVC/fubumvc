using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.NewDiagnostics
{
    public class RequestStep
    {
        public RequestStep(double requestTimeInMilliseconds, object log)
        {
            RequestTimeInMilliseconds = requestTimeInMilliseconds;
            Log = log;
            Id = Guid.NewGuid();
        }

        public double RequestTimeInMilliseconds { get; private set; }
        public object Log { get; private set; }
        public Guid Id { get; private set; }

        public bool Equals(RequestStep other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.RequestTimeInMilliseconds.Equals(RequestTimeInMilliseconds) && Equals(other.Log, Log);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(RequestStep)) return false;
            return Equals((RequestStep)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RequestTimeInMilliseconds.GetHashCode() * 397) ^ (Log != null ? Log.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("RequestTimeInMilliseconds: {0}, Log: {1}", RequestTimeInMilliseconds, Log);
        }
    }



    public class BehaviorLog : Activity
    {
        public BehaviorLog(BehaviorNode node, double start) : base(start)
        {
        }


    }

    public class PartialLog : Activity
    {
        public PartialLog(BehaviorChain chain, double start) : base(start)
        {
        }
    }

    public abstract class Activity
    {
        private readonly double _start;
        
        private double _end;
        private readonly IList<RequestStep> _steps = new List<RequestStep>();

        protected Activity(double start)
        {
            _start = start;
        }

        protected internal void AppendLog(double time, object log)
        {
            throw new NotImplementedException();
        }

        public void MarkEnd(double end)
        {
            _end = end;
        }

        public double start
        {
            get { return _start; }
        }

        public double end
        {
            get { return _end; }
        }

        public double duration
        {
            get { return _end - _start; }
        }
    }

    public interface IRequestLog
    {
        double ExecutionTime { get; }
        bool HadException { get; }


    }

    public class PerformanceHistory
    {
        public void Read(IRequestLog log)
        {
            
        }

        public long HitCount { get; private set; }
        public long ExceptionCount { get; private set; }


        public double MaxTime { get; private set; }
        public double MinTime { get; private set; }

    }


    /*
     * Subsume ExceptionHandlingObserver
     * 
     */
    public class RequestLog : Activity, IRequestLog
    {
        private readonly IDictionary<string, object> _request;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private RequestLog(IDictionary<string, object> request) : base(0)
        {
            _request = request;
            _stopwatch.Start();
        }

        public static RequestLog Start(IDictionary<string, object> request)
        {
            return new RequestLog(request);
        }

        public BehaviorChain Chain { get; set; }

        public double ExecutionTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HadException
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void LogException(Exception ex)
        {
            // would now track, don't double log this
            throw new NotImplementedException();
        }

        public void MarkFinished(Action<IDictionary<string, object>> writeResponse)
        {
            // add data to the request dict
            // stop the timer
            // log total exception time
            throw new NotImplementedException();
        }

        public void StartBehavior(BehaviorNode node)
        {
            throw new NotImplementedException();
        }

        public void FinishBehavior(BehaviorNode node)
        {
            throw new NotImplementedException();
        }

        public void StartPartial(BehaviorChain chain)
        {
            throw new NotImplementedException();
        }

        public void FinishPartial(BehaviorChain chain)
        {
            throw new NotImplementedException();
        }

        public void AddLog(object log)
        {
            throw new NotImplementedException();
        }

        // acts like the timer in diagnostics
        public void Trace(string description, Action action)
        {
            throw new NotImplementedException();
        }
    }

}