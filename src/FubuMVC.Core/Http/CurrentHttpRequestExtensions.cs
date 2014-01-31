using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Runtime.Logging;

namespace FubuMVC.Core.Http
{
    public static class CurrentHttpRequestExtensions
    {
        /// <summary>
        /// The current request matches one of these HTTP methods
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpMethods"></param>
        /// <returns></returns>
        public static bool HttpMethodMatchesAny(this ICurrentHttpRequest request, params string[] httpMethods)
        {
            return httpMethods.Any(x => x.EqualsIgnoreCase(request.HttpMethod()));
        }

        /// <summary>
        /// Evaluates if the current request is for an HTTP method *other* than the supplied httpMethods
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpMethods"></param>
        /// <returns></returns>
        public static bool IsNotHttpMethod(this ICurrentHttpRequest request, params string[] httpMethods)
        {
            return !request.HttpMethodMatchesAny(httpMethods);
        }

        /// <summary>
        /// Is an Http header present in the current request?
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasHeader(this ICurrentHttpRequest request, HttpRequestHeader key)
        {
            return request.HasHeader(HttpRequestHeaders.HeaderNameFor(key));
        }

        /// <summary>
        /// Get all values for a named header from the current request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHeader(this ICurrentHttpRequest request, HttpRequestHeader key)
        {
            return request.GetHeader(HttpRequestHeaders.HeaderNameFor(key));
        }

        /// <summary>
        /// Is the current request an Http GET?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsGet(this ICurrentHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("GET");
        }

        /// <summary>
        /// Is the current request an Http POST?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsPost(this ICurrentHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("POST");
        }

        /// <summary>
        /// Is the current request an Http HEAD?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsHead(this ICurrentHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("HEAD");
        }

        /// <summary>
        /// Is the currrent request an Http PUT?
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsPut(this ICurrentHttpRequest request)
        {
            return request.HttpMethod().EqualsIgnoreCase("PUT");
        }

        /// <summary>
        /// Converts the given url to a url relative to the current request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToRelativeContentUrl(this ICurrentHttpRequest request, string url)
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
        public static IEnumerable<string> GetDelimitedHeaderValues(this ICurrentHttpRequest request, string key)
        {
            return request.GetHeader(key).GetCommaSeparatedHeaderValues();
        }

        /// <summary>
        /// Gets the first, raw header value for the key
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSingleHeader(this ICurrentHttpRequest request, string key)
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

        public static DateTime? IfModifiedSince(this ICurrentHttpRequest request)
        {
            return request.GetSingleHeader(HttpRequestHeaders.IfModifiedSince)
                .TryParseHttpDate();
        }

        public static DateTime? IfUnModifiedSince(this ICurrentHttpRequest request)
        {
            return request.GetSingleHeader(HttpRequestHeaders.IfUnmodifiedSince)
                .TryParseHttpDate();
        }

        public static IEnumerable<string> IfMatch(this ICurrentHttpRequest request)
        {
            return request.GetHeader(HttpRequestHeaders.IfMatch).GetCommaSeparatedHeaderValues();
        }

        public static IEnumerable<string> IfNoneMatch(this ICurrentHttpRequest request)
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
    }

    public enum EtagMatch
    {
        Yes,
        No,
        None
    }

    public class CommaTokenParser
    {
        private readonly List<string> _tokens = new List<string>();
        private List<char> _characters;
        private IMode _mode;

        public CommaTokenParser()
        {
            _mode = new Searching(this);
        }

        public void Read(char c)
        {
            _mode.Read(c);
        }

        private void addChar(char c)
        {
            _characters.Add(c);
        }

        public IEnumerable<string> Tokens
        {
            get
            {
                return _tokens;
            }
        }

        private void startToken(IMode mode)
        {
            _mode = mode;
            _characters = new List<char>();
        }

        private void endToken()
        {
            var @string = new string(_characters.ToArray());
            _tokens.Add(@string);

            _mode = new Searching(this);
        }


        public interface IMode
        {
            void Read(char c);
        }

        public class Searching : IMode
        {
            private readonly CommaTokenParser _parent;

            public Searching(CommaTokenParser parent)
            {
                _parent = parent;
            }

            public void Read(char c)
            {
                if (c == ',') return;

                if (c == '"')
                {
                    _parent.startToken(new InsideQuotedToken(_parent));
                }
                else
                {
                    var normalToken = new InsideNormalToken(_parent);
                    _parent.startToken(normalToken);
                    normalToken.Read(c);
                }
            }
        }

        public class InsideQuotedToken : IMode
        {
            private readonly CommaTokenParser _parent;

            public InsideQuotedToken(CommaTokenParser parent)
            {
                _parent = parent;
            }


            public void Read(char c)
            {
                if (c == '"')
                {
                    _parent.endToken();
                }
                else
                {
                    _parent.addChar(c);
                }
            }
        }

        public class InsideNormalToken : IMode
        {
            private readonly CommaTokenParser _parent;

            public InsideNormalToken(CommaTokenParser parent)
            {
                _parent = parent;
            }

            public void Read(char c)
            {
                if (c == ',')
                {
                    _parent.endToken();
                }
                else
                {
                    _parent.addChar(c);
                }
            }
        }
    }
}