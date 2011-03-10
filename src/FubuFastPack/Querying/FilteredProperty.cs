using System.Collections.Generic;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class FilteredProperty
    {
        public Accessor Accessor { get; set; }
        public string Header { get; set; }

        public IEnumerable<StringToken> Operators { get; set; }

        public bool Equals(FilteredProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Accessor, Accessor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FilteredProperty)) return false;
            return Equals((FilteredProperty) obj);
        }

        public override int GetHashCode()
        {
            return (Accessor != null ? Accessor.GetHashCode() : 0);
        }
    }
}