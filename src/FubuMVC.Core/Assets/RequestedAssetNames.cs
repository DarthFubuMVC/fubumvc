using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public class RequestedAssetNames
    {
        public static RequestedAssetNames For(MimeType mimeType, params string[] names)
        {
            return new RequestedAssetNames{
                AssetNames = names,
                MimeType = mimeType
            };
        }

        public MimeType MimeType { get; set;}
        public IEnumerable<string> AssetNames { get; set;}

        public bool Equals(RequestedAssetNames other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.MimeType, MimeType) && other.AssetNames.IsEqualTo(AssetNames);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequestedAssetNames)) return false;
            return Equals((RequestedAssetNames) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((MimeType != null ? MimeType.GetHashCode() : 0)*397) ^ (AssetNames != null ? AssetNames.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("MimeType: {0}, AssetNames: {1}", MimeType, AssetNames.Join(", "));
        }
    }
}