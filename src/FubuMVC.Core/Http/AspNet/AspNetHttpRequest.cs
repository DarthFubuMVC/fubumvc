using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using FubuCore;
using System.Linq;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetHttpRequest : IHttpRequest
    {
        private readonly HttpRequestBase _request;
        private readonly HttpResponseBase _response;

        public AspNetHttpRequest(HttpRequestBase request, HttpResponseBase response)
        {
            _request = request;
            _response = response;
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

			if (baseUrl.IsEmpty())
			{
				baseUrl = "~/";
			}
            
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

        public bool HasHeader(string key)
        {
            var headers = _request.Headers.Get(key);
            if (headers == null) return false;

            return headers.Any();
        }

        public IEnumerable<string> GetHeader(string key)
        {
            return _request.Headers.GetValues(key) ?? new string[0];
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return _request.Headers.AllKeys;
        }

        public NameValueCollection QueryString
        {
            get
            {
                return _request.QueryString;
            }
        }

        public NameValueCollection Form
        {
            get
            {
                return _request.Form;
            }
        }

        public Stream Input
        {
            get { return _request.InputStream; }
        }

        public bool IsClientConnected()
        {
            return _response.IsClientConnected;
        }

        public ICookies Cookies
        {
            get
            {
                return new Cookies.Cookies(this);
            }
        }
    }
}