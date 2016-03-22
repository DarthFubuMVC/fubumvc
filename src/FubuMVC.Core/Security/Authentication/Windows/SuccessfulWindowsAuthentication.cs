using FubuMVC.Core.Security.Authentication.Auditing;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class SuccessfulWindowsAuthentication : AuditMessage
    {
        public string User { get; set; }

        protected bool Equals(SuccessfulWindowsAuthentication other)
        {
            return string.Equals(User, other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SuccessfulWindowsAuthentication) obj);
        }

        public override int GetHashCode()
        {
            return (User != null ? User.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("User: {0}", User);
        }
    }

    public class FailedWindowsAuthentication : AuditMessage
    {
        public string User { get; set; }

        protected bool Equals(FailedWindowsAuthentication other)
        {
            return string.Equals(User, other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FailedWindowsAuthentication) obj);
        }

        public override int GetHashCode()
        {
            return (User != null ? User.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Failed windows authentication with: {0}", User);
        }
    }
}