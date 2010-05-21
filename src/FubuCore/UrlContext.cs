using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FubuCore
{
    public static class UrlContext
    {
        static UrlContext()
        {
            Reset();
        }

        private static Func<string, string, string> _combine { get; set; }
        private static Func<string, string> _toAbsolute { get; set; }
        private static Func<string, bool> _isAbsolute { get; set; }
        private static Func<string, string> _mapPath { get; set; }

        public static void Reset()
        {
            if (HttpRuntime.AppDomainAppVirtualPath != null)
            {
                Live();
                return;
            }

            Stub("");
        }

        public static void Stub()
        {
            Stub("");
        }

        public static void Stub(string usingFakeUrl)
        {
            _combine = (basePath, subPath) => "{0}/{1}".ToFormat(basePath.TrimEnd('/'), subPath.TrimStart('/'));
            _isAbsolute = path => path.StartsWith("/");
            _toAbsolute = path => _isAbsolute(path) ? path : _combine(usingFakeUrl, path.Replace("~", ""));
            _mapPath = virtPath => _toAbsolute(virtPath).Replace("~", "").Replace("//", "/").Replace("/", "\\");
        }

        public static void Live()
        {
            _combine = VirtualPathUtility.Combine;
            _toAbsolute = VirtualPathUtility.ToAbsolute;
            _isAbsolute = VirtualPathUtility.IsAbsolute;
            _mapPath = HostingEnvironment.MapPath;
        }

        public static string ToAbsoluteUrl(this string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;
            if (!_isAbsolute(url))
            {
                url = _combine("~", url);
            }
            return _toAbsolute(url);
        }

        public static string ToServerQualifiedUrl(this string relativeUrl, string serverBasedUrl)
        {
            var baseUri = new Uri(serverBasedUrl);
            return new Uri(baseUri, ToAbsoluteUrl(relativeUrl)).ToString();
        }

        public static string ToPhysicalPath(this string webRelativePath)
        {
            if (!_isAbsolute(webRelativePath))
            {
                webRelativePath = _combine("~", webRelativePath);
            }
            return _mapPath(webRelativePath);
        }

        public static string WithQueryStringValues(this string querystring, params object[] values)
        {
            return querystring.ToFormat(values.Select(value => value.ToString().UrlEncoded()).ToArray());
        }

        public static string UrlEncoded(this object target)
        {
            //properly encoding URI: http://blogs.msdn.com/yangxind/default.aspx
            return target != null ? Uri.EscapeDataString(target.ToString()) : string.Empty;
        }
    }
}