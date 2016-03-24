using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Validation.Web.UI
{
	public class FieldOptions
	{
		public FieldOptions()
		{
			rules = new FieldRuleOptions[0];
		}

		public string field { get; set; }
		public string mode { get; set; }
		public FieldRuleOptions[] rules { get; set; }

		public override string ToString()
		{
			return "{0} ({1})".ToFormat(field, mode);
		}

		protected bool Equals(FieldOptions other)
		{
			return string.Equals(field, other.field) && string.Equals(mode, other.mode)
				&& rules.SequenceEqual(other.rules);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((FieldOptions) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (field.GetHashCode()*397) ^ mode.GetHashCode();
			}
		}
	}

	public class FieldRuleOptions
	{
		public string rule { get; set; }
		public string mode { get; set; }

		public override string ToString()
		{
			return "{0} ({1})".ToFormat(rule, mode);
		}

		protected bool Equals(FieldRuleOptions other)
		{
			return string.Equals(rule, other.rule) && string.Equals(mode, other.mode);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((FieldRuleOptions)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (rule.GetHashCode() * 397) ^ mode.GetHashCode();
			}
		}
	}
}