using System;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.UI.Security
{
    [Serializable]
    public class AccessRight : IComparable<AccessRight>, IEquatable<AccessRight>
    {
        private readonly bool _read;
        private readonly bool _write;
        private readonly int _permissiveness;
        private readonly string _name;

        private AccessRight(bool read, bool write, int permissiveness, string name)
        {
            _read = read;
            _write = write;
            _permissiveness = permissiveness;
            _name = name;
        }

        public int Permissiveness
        {
            get { return _permissiveness; }
        }

        public bool Read
        {
            get { return _read; }
        }

        public bool Write
        {
            get { return _write; }
        }

        public string Name
        {
            get { return _name; }
        }

        public static readonly AccessRight All = new AccessRight(true, true, 1, "All");
        public static readonly AccessRight ReadOnly = new AccessRight(true, false, 2, "ReadOnly");
        public static readonly AccessRight None = new AccessRight(false, false, 3, "None");

        public static AccessRight operator +(AccessRight first, AccessRight second)
        {
            return first.Permissiveness <= second.Permissiveness ? first : second;
        }

        public static bool operator >(AccessRight first, AccessRight second)
        {
            return first.Permissiveness < second.Permissiveness;
        }
        
        public static bool operator >=(AccessRight first, AccessRight second)
        {
            return first.Permissiveness <= second.Permissiveness;
        }

        public static bool operator <(AccessRight first, AccessRight second)
        {
            return first.Permissiveness > second.Permissiveness;
        }

        public static bool operator <=(AccessRight first, AccessRight second)
        {
            return first.Permissiveness >= second.Permissiveness;
        }

        public bool Equals(AccessRight other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return _permissiveness;
        }

        public static AccessRight Least(params AccessRight[] rights)
        {
            if (!rights.Any()) return None;
            return rights.Min();
        }

        public static AccessRight Most(params AccessRight[] rights)
        {
            if (!rights.Any()) return None;
            return rights.Max();
        }

        public int CompareTo(AccessRight other)
        {
            return Permissiveness.CompareTo(other.Permissiveness) * -1;
        }

        public override string ToString()
        {
            return string.Format("Read: {0}, Write: {1}", _read, _write);
        }

        public static AccessRight For(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (name.EqualsIgnoreCase(All.Name)) return All;
            if (name.EqualsIgnoreCase(ReadOnly.Name)) return ReadOnly;
            if (name.EqualsIgnoreCase(None.Name)) return None;
            throw new ArgumentOutOfRangeException("name", name, "All, ReadOnly, and None are the only valid options");
        }
    }
}