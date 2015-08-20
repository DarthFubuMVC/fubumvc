using System;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class RequestStep
    {
        public RequestStep(double requestTime, object log)
        {
            RequestTime = requestTime;
            Log = log;
            Id = Guid.NewGuid();
        }

        public double RequestTime { get; private set; }
        public object Log { get; private set; }

        public Guid Id { get; private set; }
        public Activity Activity { get; set; }

        public bool Equals(RequestStep other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.RequestTime.Equals(RequestTime) && Equals(other.Log, Log);
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
                return (RequestTime.GetHashCode() * 397) ^ (Log != null ? Log.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("RequestTimeInMilliseconds: {0}, Log: {1}", RequestTime, Log);
        }
    }
}