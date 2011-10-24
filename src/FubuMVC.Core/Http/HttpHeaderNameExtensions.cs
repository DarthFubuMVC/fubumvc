using System;
using System.Net;

namespace FubuMVC.Core.Http
{
    public static class HttpHeaderNameExtensions
    {
        public static void Value<T>(this IRequestHeaders headers, HttpRequestHeader header, Action<T> callback)
        {
            headers.Value(header.ToName(), callback);
        }

        public static string ToName(this HttpRequestHeader header)
        {
            return HttpRequestHeaders.HeaderNameFor(header);
        }

        public static string ToName(this HttpResponseHeader header)
        {
            return HttpResponseHeaders.HeaderNameFor(header);
        }
    }
}