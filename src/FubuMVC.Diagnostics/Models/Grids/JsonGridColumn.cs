namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridColumn
    {
        public JsonGridColumn()
        {
            Value = string.Empty;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; }
        public bool IsHidden { get; set; }
        public bool HideFilter { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (JsonGridColumn)) return false;
			return Equals((JsonGridColumn) obj);
		}

    	public bool Equals(JsonGridColumn other)
    	{
    		if (ReferenceEquals(null, other)) return false;
    		if (ReferenceEquals(this, other)) return true;
    		return Equals(other.Name, Name) && Equals(other.Value, Value);
    	}

    	public override int GetHashCode()
    	{
    		unchecked
    		{
    			return (Name.GetHashCode()*397) ^ Value.GetHashCode();
    		}
    	}
    }
}