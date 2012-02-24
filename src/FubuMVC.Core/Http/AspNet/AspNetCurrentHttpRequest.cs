using System;
using System.Net;
using System.Web;
using FubuCore;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly HttpRequestBase _request;

        public AspNetCurrentHttpRequest(HttpRequestBase request)
        {
            _request = request;
        }

        public string RawUrl()
        {
            return _request.RawUrl;
        }

        public string RelativeUrl()
        {
            return _request.AppRelativeCurrentExecutionFilePath;
        }

        public string FullUrl()
        {
            return _request.Url.ToString();
        }

        public string ToFullUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;
            if (url.IsEmpty()){ url = "~/"; }

            var urlParts = url.Split(new[] { '?' }, 2);
            var baseUrl = urlParts[0];
            
            if (!VirtualPathUtility.IsAbsolute(baseUrl))
            {
                baseUrl = VirtualPathUtility.Combine("~", baseUrl);
            }

            var absoluteUrl = VirtualPathUtility.ToAbsolute(baseUrl);
            
            if (urlParts.Length > 1) absoluteUrl += ("?" + urlParts[1]);
            return absoluteUrl;
        }

        public string HttpMethod()
        {
            return _request.HttpMethod;
        }

        public T GetHeader<T>(HttpRequestHeader header)
        {
            throw new NotImplementedException();
        }

        public T GetHeader<T>(string headerName)
        {
            throw new NotImplementedException();
        }
    }
}