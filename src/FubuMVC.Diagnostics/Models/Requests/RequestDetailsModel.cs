using System;
using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;

namespace FubuMVC.Diagnostics.Models.Requests
{
	public class RequestDetailsModel
	{
		// Leave this here for extensibility
		public IDebugReport Report { get; set; }

		public BehaviorDetailsModel Root { get; set; }
	}

	public class BehaviorDetailsModel
	{
		private readonly IList<IBehaviorDetails> _before = new List<IBehaviorDetails>();
		private readonly IList<IBehaviorDetails> _after = new List<IBehaviorDetails>();

		public Type BehaviorType { get; set; }
		public IEnumerable<IBehaviorDetails> Before { get { return _before; } }
		public BehaviorDetailsModel Inner { get; set; }
		public IEnumerable<IBehaviorDetails> After { get { return _after; } }

		public void AddBefore(IBehaviorDetails details)
		{
			_before.Fill(details);
		}

		public void AddAfter(IBehaviorDetails details)
		{
			_after.Fill(details);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (BehaviorDetailsModel)) return false;
			return Equals((BehaviorDetailsModel) obj);
		}

		public bool Equals(BehaviorDetailsModel other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.BehaviorType, BehaviorType);
		}

		public override int GetHashCode()
		{
			return BehaviorType.GetHashCode();
		}
	}
}