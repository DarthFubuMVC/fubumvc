using System;

namespace Bottles.DependencyAnalysis
{
    public class Urn
    {
        readonly Uri _uri;

        public Urn(string urn):
            this(new Uri(urn))
        {
            
        }
        public Urn(Uri uri)
        {
            _uri = uri;
        }

        public bool Equals(Urn other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._uri, _uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Urn)) return false;
            return Equals((Urn) obj);
        }

        public override int GetHashCode()
        {
            return (_uri != null ? _uri.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _uri.ToString();
        }
    }
}