using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public class SetContentType : IRecordedHttpOutput, DescribesItself
    {
        private readonly string _contentType;

        public SetContentType(string contentType)
        {
            _contentType = contentType;
        }

        public void Replay(IHttpResponse response)
        {
            response.WriteContentType(_contentType);
        }

        public bool Equals(SetContentType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._contentType, _contentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SetContentType)) return false;
            return Equals((SetContentType) obj);
        }

        public override int GetHashCode()
        {
            return (_contentType != null ? _contentType.GetHashCode() : 0);
        }

        public void Describe(Description description)
        {
            description.Title = "ContentType is " + _contentType;
        }

        public override string ToString()
        {
            return string.Format("ContentType: {0}", _contentType);
        }
    }
}