using System;

namespace FubuMVC.Core.Diagnostics.Runtime
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
            if (obj.GetType() != typeof (RequestStep)) return false;
            return Equals((RequestStep) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RequestTimeInMilliseconds.GetHashCode()*397) ^ (Log != null ? Log.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("RequestTimeInMilliseconds: {0}, Log: {1}", RequestTimeInMilliseconds, Log);
        }
    }
}