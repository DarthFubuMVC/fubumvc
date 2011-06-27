using System;

namespace FubuMVC.Core.Diagnostics
{
    public class RedirectReport : IBehaviorDetails
    {
        public string Url;

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.Redirect(this);
        }

        public bool Equals(RedirectReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Url, Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RedirectReport)) return false;
            return Equals((RedirectReport) obj);
        }

        public override int GetHashCode()
        {
            return (Url != null ? Url.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Url: {0}", Url);
        }
    }

    public abstract class BehaviorIndicator : IBehaviorDetails
    {
        public Type BehaviorType { get; set; }

        public abstract void AcceptVisitor(IBehaviorDetailsVisitor visitor);

        public bool Equals(BehaviorStart other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj.GetType() == typeof(BehaviorStart);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class BehaviorStart : BehaviorIndicator
    {
        public override void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
        }
    }

    public class BehaviorFinish : BehaviorIndicator
    {
        public override void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
        }
    }
}