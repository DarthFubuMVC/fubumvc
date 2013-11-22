using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;

namespace FubuMVC.Core.Http
{
    public static class CurrentHttpRequestExtensions
    {
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

    }
}