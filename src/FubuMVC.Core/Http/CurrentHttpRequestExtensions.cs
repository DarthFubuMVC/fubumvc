using System.Collections.Generic;
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

    }
}