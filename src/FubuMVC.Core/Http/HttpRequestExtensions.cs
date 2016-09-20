using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime.Logging;

namespace FubuMVC.Core.Http
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// The current request matches one of these HTTP methods
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpMethods"></param>
        /// <returns></returns>
        public static bool HttpMethodMatchesAny(this IHttpRequest request, params string[] httpMethods)
        {
            return httpMethods.Any(x => x.EqualsIgnoreCase(request.HttpMethod()));
        }

        /// <summary>
        /// Evaluates if the current request is for an HTTP method *other* than the supplied httpMethods
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpMethods"></param>
        /// <returns></returns>
        public static bool IsNotHttpMethod(this IHttpRequest request, params string[] httpMethods)
        {
            return !request.HttpMethodMatchesAny(httpMethods);
        }

        /// <summary>
        /// Is an Http header present in the current request?
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasHeader(this IHttpRequest request, HttpRequestHeader key)
        {
            return request.HasHeader(HttpRequestHeaders.HeaderNameFor(key));
        }

        /// <summary>
        /// Get all values for a named header from the current request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHeader(this IHttpRequest request, HttpRequestHeader key)
        {
            return request.GetHeader(HttpRequestHeaders.HeaderNameFor(key));
        }

        /// <summary>
        /// Is the current request an Http GET?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsGet(this IHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("GET");
        }

        /// <summary>
        /// Is the current request an Http POST?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsPost(this IHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("POST");
        }

        /// <summary>
        /// Is the current request an Http HEAD?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsHead(this IHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("HEAD");
        }

        /// <summary>
        /// Is the currrent request an Http PUT?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsPut(this IHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("PUT");
        }

        /// <summary>
        /// Converts the given url to a url relative to the current request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToRelativeContentUrl(this IHttpRequest request, string url)
        {
            var current = request.RelativeUrl().TrimStart('/');
            var contentUrl = url.TrimStart('/');

            if (current == string.Empty)
            {
                return contentUrl;
            }

            if (contentUrl.StartsWith(current))
            {
                return contentUrl.Substring(current.Length).TrimStart('/');
            }

            var prepend = current.Split('/').Select(x => "..").Join("/");
            var relativeUrl = prepend.AppendUrl(contentUrl);

            return relativeUrl;
        }

        /// <summary>
        /// Get the associated values from the collection separated into individual values.
        /// Quoted values will not be split, and the quotes will be removed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDelimitedHeaderValues(this IHttpRequest request, string key)
        {
            return request.GetHeader(key).GetCommaSeparatedHeaderValues();
        }

        /// <summary>
        /// Gets the first, raw header value for the key
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSingleHeader(this IHttpRequest request, string key)
        {
            return request.GetHeader(key).FirstOrDefault();
        }

        public static IEnumerable<string> GetCommaSeparatedHeaderValues(this IEnumerable<string> enumerable)
        {
            foreach (var content in enumerable)
            {
                var searchString = content.Trim();
                if (searchString.Length == 0) break;

                var parser = new CommaTokenParser();
                content.ToCharArray().Each(parser.Read);

                // Gotta force the parser to know it's done
                parser.Read(',');

                foreach (var token in parser.Tokens)
                {
                    yield return token.Trim();
                }
            }


        }

        public static string Quoted(this string value)
        {
            return "\"{0}\"".ToFormat(value);
        }

        
        // TODO -- will want a ReplaceValue too
        public static void AppendValue(this IDictionary<string, string[]> headers, string key, string value)
        {
            if (headers.ContainsKey(key))
            {
                var oldArray = headers[key];
                var newArray = new string[oldArray.Length + 1];
                oldArray.CopyTo(newArray, 0);
                newArray[oldArray.Length] = value;

                headers[key] = newArray;
            }
            else
            {
                headers[key] = new[] { value };
            }
        }

        public static DateTime? TryParseHttpDate(this string dateString)
        {
            DateTime date;

            return DateTime.TryParseExact(dateString, "r", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                ? date
                : null as DateTime?;
        }

        public static string ToHttpDateString(this DateTime time)
        {
            return time.ToUniversalTime().ToString("r");
        }

        public static DateTime? IfModifiedSince(this IHttpRequest request)
        {
            return request.GetSingleHeader(HttpRequestHeaders.IfModifiedSince)
                .TryParseHttpDate();
        }

        public static DateTime? IfUnModifiedSince(this IHttpRequest request)
        {
            return request.GetSingleHeader(HttpRequestHeaders.IfUnmodifiedSince)
                .TryParseHttpDate();
        }

        public static IEnumerable<string> IfMatch(this IHttpRequest request)
        {
            return request.GetHeader(HttpRequestHeaders.IfMatch).GetCommaSeparatedHeaderValues();
        }

        public static IEnumerable<string> IfNoneMatch(this IHttpRequest request)
        {
            return request.GetHeader(HttpRequestHeaders.IfNoneMatch).GetCommaSeparatedHeaderValues();
        }

        public static EtagMatch EtagMatches(this IEnumerable<string> values, string etag)
        {
            if (values == null || !values.Any()) return EtagMatch.None;

            return values.Any(x => x.Equals(etag, StringComparison.Ordinal) || x == "*")
                ? EtagMatch.Yes
                : EtagMatch.No;

        }

        /// <summary>
        /// Checks whether or not there is any data in the request body
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool HasBodyData(this IHttpRequest data)
        {
            return data.Input != null && data.Input.CanRead && data.Input.Length > 0;
        }

        public static bool CouldBeJson(this IHttpRequest data)
        {
            if (!data.HasBodyData()) return false;

            var reader = new StreamReader(data.Input);
            var firstCharacter = reader.Read();
            data.Input.Position = 0;

            return firstCharacter == '{';
        }

        public static readonly string LOG_KEY = "fubu.ExecutionLog";
        public static readonly string REQUEST_ID = "x-request-id";

        public static void Set<T>(this IDictionary<string, object> dict, string key, T value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static string RequestId(this IDictionary<string, object> http)
        {
            var raw = http.ResponseHeaders();

            if (raw == null) return null;

            if (raw.ContainsKey(REQUEST_ID))
            {
                return raw[REQUEST_ID].FirstOrDefault();
            }

            return null;

        }

        public static void RequestId(this IDictionary<string, object> http, string id)
        {
            http.ResponseHeaders().AppendValue(REQUEST_ID, id);
        }

        public static string RequestId(this IHttpResponse response)
        {
            var raw = response.HeaderValueFor(REQUEST_ID);
            if (raw == null || !raw.Any()) return null;

            return raw.First();
        }

        public static void CopyTo(this IDictionary<string, object> source, IDictionary<string, object> destination,
            params string[] keys)
        {
            keys.Where(source.ContainsKey).Each(x => destination.Add(x, source[x]));
        }

        public static IDictionary<string, string[]> ResponseHeaders(this IDictionary<string, object> dict)
        {
            if (!dict.ContainsKey(OwinConstants.ResponseHeadersKey))
            {
                dict.Add(OwinConstants.ResponseHeadersKey, new Dictionary<string, string[]>());
            }

            return dict[OwinConstants.ResponseHeadersKey].As<IDictionary<string, string[]>>();
        } 

        public static void Log(this IDictionary<string, object> dict, IChainExecutionLog log)
        {
            dict.ResponseHeaders().AppendValue(REQUEST_ID, log.Id.ToString());
            dict.Set(LOG_KEY, log);
        }

        public static IChainExecutionLog Log(this IDictionary<string, object> dict)
        {
            return (IChainExecutionLog) (dict.ContainsKey(LOG_KEY) ? dict[LOG_KEY] : null);
        }

    }
}