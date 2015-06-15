using System;
using FubuCore;

namespace Serenity
{
    public static class UrlExtensions
    {
        public static bool IsUrlMatch(this string browserUrl, string url)
        {
            if (url.IsEmpty()) return false;

            return browserUrl.Canonize().EqualsIgnoreCase(url.Canonize());
        }

        public static string Canonize(this string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var uri = new Uri(url);
                return uri.PathAndQuery;
            }

            return url;
        }
    }
}