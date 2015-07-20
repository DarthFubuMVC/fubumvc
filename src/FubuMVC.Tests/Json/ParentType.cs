namespace FubuMVC.Tests.Json
{
	public class ParentType
	{
		public string Name { get; set; }
		public ComplexType Child { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ParentType)) return false;
			return Equals((ParentType) obj);
		}

		public bool Equals(ParentType other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, Name) && Equals(other.Child, Child);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Name.GetHashCode()*397) ^ Child.GetHashCode();
			}
		}
	}
}