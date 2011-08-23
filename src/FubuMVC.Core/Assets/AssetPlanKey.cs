using System;
using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public class AssetPlanKey
    {
        private readonly MimeType _mimeType;
        private readonly AssetNamesKey _names;

        public AssetPlanKey(MimeType mimeType, IEnumerable<string> names)
        {
            _mimeType = mimeType;
            _names = new AssetNamesKey(names);
        }

        public MimeType MimeType
        {
            get { return _mimeType; }
        }

        public IEnumerable<string> Names
        {
            get { return _names.Names; }
        }

        public bool Equals(AssetPlanKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._mimeType, _mimeType) && Equals(other._names, _names);
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
                return ((_mimeType != null ? _mimeType.GetHashCode() : 0)*397) ^ (_names != null ? _names.GetHashCode() : 0);
            }
        }

        public static AssetPlanKey For(MimeType mimeType, params string[] names)
        {
            return new AssetPlanKey(mimeType, names);
        }

        public override string ToString()
        {
            return string.Format("MimeType: {0}, Names: {1}", _mimeType, Names.Join(", "));
        }
    }
}