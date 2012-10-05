using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuCore;

namespace FubuMVC.Core.Assets
{
    public class AssetNamesKey
    {
        private readonly IEnumerable<string> _names;
        private readonly Lazy<int> _hashcode;

        public AssetNamesKey(IEnumerable<string> names)
        {
            _names = names.OrderBy(x => x);

            _hashcode = new Lazy<int>(() =>
            {
                var combined = names.Join("*");
                return combined.ToHash().GetHashCode();
            });
        }

        public IEnumerable<string> Names
        {
            get { return _names; }
        }

        public bool Equals(AssetNamesKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other._names.IsEqualTo(_names);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssetNamesKey)) return false;
            return Equals((AssetNamesKey) obj);
        }

        public override int GetHashCode()
        {
            return _hashcode.Value;
        }

    }
}