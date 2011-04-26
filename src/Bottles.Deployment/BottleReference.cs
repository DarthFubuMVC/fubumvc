namespace Bottles.Deployment
{
    public class BottleReference
    {
        public string Name { get; set; }
        public string Relationship { get; set; }

        public BottleReference()
        {
        }

        public BottleReference(string name, string relationship)
        {
            Name = name;
            Relationship = relationship;
        }

        public bool Equals(BottleReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Relationship, Relationship);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BottleReference)) return false;
            return Equals((BottleReference) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Relationship != null ? Relationship.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Relationship: {1}", Name, Relationship);
        }
    }
}