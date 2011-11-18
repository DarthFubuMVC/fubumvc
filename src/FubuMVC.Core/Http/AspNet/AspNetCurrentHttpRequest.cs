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
            return _request.PathInfo;
        }

        public string ToFullUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;
            if (url.IsEmpty())
            {
                url = "~/";
            }
            else
            {
                url = "~/" + url.TrimStart('/');
            }

            var absoluteUrl = VirtualPathUtility.ToAbsolute(url);
            
            return (absoluteUrl != "/") ? absoluteUrl.TrimEnd('/') : absoluteUrl;
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