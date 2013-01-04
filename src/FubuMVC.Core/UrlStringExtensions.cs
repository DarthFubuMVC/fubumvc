using System;
using System.Web;
using System.Linq;

namespace FubuMVC.Core
{
    using FubuCore;

    public static class UrlStringExtensions
    {
        public static string HtmlAttributeEncode(this string unEncoded)
        {
            return HttpUtility.HtmlAttributeEncode(unEncoded);
        }

        public static string HtmlEncode(this string unEncoded)
        {
            return HttpUtility.HtmlEncode(unEncoded);
        }

        public static string HtmlDecode(this string encoded)
        {
            return HttpUtility.HtmlDecode(encoded);
        }

        public static string UrlEncode(this string unEncoded)
        {
            return HttpUtility.UrlEncode(unEncoded);
        }

        public static string UrlDecode(this string encoded)
        {
            return HttpUtility.UrlDecode(encoded);
        }

        public static string ToAbsoluteUrl(this string url, string applicationUrl)
        {
            

            if (!Uri.IsWellFormedUriString(applicationUrl, UriKind.Absolute))
            {
                throw new ArgumentOutOfRangeException("applicationUrl", "applicationUrl must be an absolute url");
            }

            if (url.Contains("://")) return url;

            
            url = url ?? string.Empty;
            url = url.TrimStart('~', '/').TrimStart('/');

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;

            return (applicationUrl.TrimEnd('/') + "/" + url).TrimEnd('/');
        }

        public static string ToServerQualifiedUrl(this string relativeUrl, string serverBasedUrl)
        {
            var baseUri = new Uri(serverBasedUrl);
            return new Uri(baseUri, relativeUrl.ToAbsoluteUrl(serverBasedUrl)).ToString();
        }

        public static string UrlEncoded(this object target)
        {
            //properly encoding URI: http://blogs.msdn.com/yangxind/default.aspx
            return target != null ? Uri.EscapeDataString(target.ToString()) : string.Empty;
        }

        public static string WithQueryStringValues(this string querystring, params object[] values)
        {
            return querystring.ToFormat(values.Select(value => value.ToString().UrlEncoded()).ToArray());
        }

        public static string WithoutQueryString(this string querystring)
        {
            var questionMarkIndex = querystring.IndexOf('?');

            if (questionMarkIndex == -1) return querystring;

            return querystring.Substring(0, questionMarkIndex);
        }
    }
}