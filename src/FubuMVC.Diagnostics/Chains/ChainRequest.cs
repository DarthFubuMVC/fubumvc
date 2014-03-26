using System;

namespace FubuMVC.Diagnostics.Chains
{
	public class ChainRequest
	{
		public Guid Id { get; set; }

	    public bool Equals(ChainRequest other)
	    {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return other.Id.Equals(Id);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != typeof (ChainRequest)) return false;
	        return Equals((ChainRequest) obj);
	    }

	    public override int GetHashCode()
	    {
	        return Id.GetHashCode();
	    }

	    public override string ToString()
	    {
	        return string.Format("Id: {0}", Id);
	    }
	}
}