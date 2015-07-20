using FubuCore;

namespace FubuMVC.Tests.Json
{
	public class ComplexType
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public override string ToString()
		{
			return "{0}:{1}".ToFormat(Key, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ComplexType)) return false;
			return Equals((ComplexType) obj);
		}

		public bool Equals(ComplexType other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Key, Key) && Equals(other.Value, Value);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Key.GetHashCode()*397) ^ Value.GetHashCode();
			}
		}
	}
}