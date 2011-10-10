using System;
using System.Collections.Generic;
using System.Web;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public static class AjaxExtensions
    {
        public const string XmlHttpRequestValue = "XMLHttpRequest";
        public const string XRequestedWithHeader = "X-Requested-With";

        public static bool IsAjaxRequest(this HttpContext context)
        {
            var wrapper = new HttpContextWrapper(context);
            AggregateDictionary dictionary = AggregateDictionary.ForHttpContext(wrapper);
            return new RequestData(dictionary).IsAjaxRequest();
        }

        public static bool IsAjaxRequest(this IDictionary<string, object> requestInput)
        {
            object value;
            return
                requestInput.TryGetValue(XRequestedWithHeader, out value)
                && IsAjaxRequest(value);
        }

        public static bool IsAjaxRequest(this IRequestData requestInput)
        {
            bool result = false;
            requestInput.Value(XRequestedWithHeader, value => result = IsAjaxRequest(value));
            return result;
        }

        private static bool IsAjaxRequest(this object value)
        {
            return XmlHttpRequestValue.Equals(value as string, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsAjaxRequest(this IBindingContext context)
        {
            var returnValue = false;
            context.ValueAs<object>(XRequestedWithHeader, val => returnValue = val.IsAjaxRequest());
            return returnValue;
        }

    }
}