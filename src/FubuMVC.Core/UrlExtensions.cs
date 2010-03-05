using System;
using System.Collections;
using System.Diagnostics;
using FubuCore;

namespace FubuMVC.Core
{
    public static class UrlExtensions
    {
        public static string ToFullUrl(this string relativeUrl, params object[] args)
        {
            string formattedUrl = (args == null) ? relativeUrl : relativeUrl.ToFormat(args);

            return UrlContext.GetFullUrl(formattedUrl);
        }

        public static string UrlEncoded(this object target)
        {
            //properly encoding URI: http://blogs.msdn.com/yangxind/default.aspx
            return target != null ? Uri.EscapeDataString(target.ToString()) : string.Empty;
        }
    }
}