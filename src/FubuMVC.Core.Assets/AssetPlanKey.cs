using System;
using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public class AssetPlanKey : IEnumerable<string>
    {
        private readonly MimeType _mimeType;
        private readonly IEnumerable<string> _names;

        public AssetPlanKey(MimeType mimeType, IEnumerable<string> names)
        {
            _mimeType = mimeType;
            _names = names;
        }

        public MimeType MimeType
        {
            get { return _mimeType; }
        }

        public IEnumerable<string> Names
        {
            get { return _names; }
        }

        public bool Equals(AssetPlanKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._mimeType, _mimeType) && other._names.IsEqualTo(_names);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssetPlanKey)) return false;
            return Equals((AssetPlanKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_mimeType != null ? _mimeType.GetHashCode() : 0)*397) ^ (_names != null ? _names.Join("*").GetHashCode() : 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static AssetPlanKey For(MimeType mimeType, params string[] names)
        {
            return new AssetPlanKey(mimeType, names);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _names.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("MimeType: {0}, Names: {1}", _mimeType, Names.Join(", "));
        }
    }
}