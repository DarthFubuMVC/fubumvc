using System;
using System.Collections.Generic;
using System.Net;
using FubuCore.Descriptions;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Http.Headers
{
    [Title("Http Header")]
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

        // for serialization
        public Header()
        {
        }

        public void Write(IOutputWriter writer)
        {
            writer.AppendHeader(Name, Value);
        }

        public void Replay(IHttpResponse response)
        {
            response.AppendHeader(Name, Value);
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

    public static class HeadersExtension
    {
        public static string ValueFor(this IEnumerable<Header> headers,  HttpRequestHeader header)
        {
            return headers.ValueFor(HttpRequestHeaders.HeaderNameFor(header));
        }

        public static string ValueFor(this IEnumerable<Header> headers, HttpResponseHeader header)
        {
            return headers.ValueFor(HttpResponseHeaders.HeaderNameFor(header));
        }

        public static string ValueFor(this IEnumerable<Header> headers, string headerName)
        {
            var header = headers.FirstOrDefault(x => x.Matches(headerName));
            return header == null ? null : header.Value;
        }


    }
}