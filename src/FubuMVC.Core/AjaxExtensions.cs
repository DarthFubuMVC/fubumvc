using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core
{
    public static class AjaxExtensions
    {
        public const string XmlHttpRequestValue = "XMLHttpRequest";
        public const string XRequestedWithHeader = "X-Requested-With";

        /// <summary>
        /// Tries to determine whether or not a given request is an Ajax request by looking for the "X-Requested-With" header
        /// </summary>
        /// <param name="requestInput"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this IDictionary<string, object> requestInput)
        {
            object value;
            return
                requestInput.TryGetValue(XRequestedWithHeader, out value)
                && IsAjaxRequest(value);
        }

        /// <summary>
        /// Tries to determine whether or not a given request is an Ajax request by looking for the X-Requested-With" header
        /// </summary>
        /// <param name="requestInput"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this IRequestData requestInput)
        {
            bool result = false;
            requestInput.Value(XRequestedWithHeader, value => result = IsAjaxRequest(value.RawValue));
            return result;
        }

        /// <summary>
        /// Determines whether or not a request is an Ajax request by comparing the value to "XMLHttpRequest"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsAjaxRequest(this object value)
        {
            return XmlHttpRequestValue.Equals(value as string, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether or not a request is an Ajax request by searching for a value of the "X-Requested-With" header
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this IBindingContext context)
        {
            bool returnValue = false;
            context.Data.ValueAs<object>(XRequestedWithHeader, val => returnValue = val.IsAjaxRequest());
            return returnValue;
        }

        /// <summary>
        /// Determines whether or not a request is an Ajax request by searching for a value of the "X-Requested-With" header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this IHttpRequest request)
        {
            var headers = request.GetHeader(XRequestedWithHeader);
            return headers.Any(x => x.EqualsIgnoreCase(XmlHttpRequestValue));
        }
    }
}