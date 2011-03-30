using System.Net;

namespace FubuMVC.Core.Diagnostics
{
	public class HttpStatusReport : IBehaviorDetails
	{
		public HttpStatusCode Status { get; set; }

		public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
		{
			visitor.HttpStatus(this);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(HttpStatusReport)) return false;
			return Equals((HttpStatusReport)obj);
		}

		public bool Equals(HttpStatusReport other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Status, Status);
		}

		public override int GetHashCode()
		{
			return Status.GetHashCode();
		}
	}
}