using System;
using System.Net;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Headers
{
    public class Header : IRecordedHttpOutput
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Header(HttpResponseHeader header, string value) : this(HttpResponseHeaders.HeaderNameFor(header), value)
        {
            
        }

        public void Write(IOutputWriter writer)
        {
            writer.AppendHeader(Name, Value);
        }

        public void Replay(IHttpWriter writer)
        {
            writer.AppendHeader(Name, Value);
        }


        public bool Equals(Header other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Header)) return false;
            return Equals((Header) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }


        public override string ToString()
        {
            return string.Format("Name: {0}, Value: {1}", Name, Value);
        }

        public bool Matches(string headerName)
        {
            return string.Equals(Name, headerName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}