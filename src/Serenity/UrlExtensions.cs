using System;
using System.Linq;
using FubuCore;

namespace Serenity
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Does the absolute path *minus* the querystring of the two url's match
        /// </summary>
        /// <param name="browserUrl"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool Matches(this string browserUrl, string url)
        {
            if (url.IsEmpty()) return false;

            return browserUrl.Canonize().EqualsIgnoreCase(url.Canonize());
        }

        /// <summary>
        /// Does the absolute path including the querystring of the two url's match
        /// </summary>
        /// <param name="browserUrl"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool MatchesWithQuerystring(this string browserUrl, string url)
        {
            if (url.IsEmpty()) return false;

            return browserUrl.CanonizeExact().EqualsIgnoreCase(url.CanonizeExact());
        }

        public static string CanonizeExact(this string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var uri = new Uri(url);
                return uri.PathAndQuery;
            }

            return url;
        }

        public static string Canonize(this string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var uri = new Uri(url);
                return uri.AbsolutePath;
            }
            else
            {
                var parts = url.Split('?');
                return parts.First();
            }
        }
    }
}