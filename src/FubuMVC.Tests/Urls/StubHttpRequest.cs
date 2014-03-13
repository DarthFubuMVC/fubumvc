using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Tests.Urls
{

    public class StubHttpRequest : IHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public StubHttpRequest()
        {
            QueryString = new NameValueCollection();
        }

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string FullUrl()
        {
            return StubFullUrl;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(TheApplicationRoot);
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }

        public Cache<string, string[]> Headers = new Cache<string, string[]>(key => new string[0]); 

        public bool HasHeader(string key)
        {
            return Headers.Has(key);
        }

        public IEnumerable<string> GetHeader(string key)
        {
            return Headers[key];
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return Headers.GetAllKeys();
        }

        public NameValueCollection QueryString { get; private set; }
        public NameValueCollection Form { get; private set; }
        public Stream Input { get; private set; }
        public bool IsClientConnected()
        {
            throw new NotImplementedException();
        }

        public ICookies Cookies
        {
            get
            {
                return new Core.Http.Cookies.Cookies(this);
            }
        }
    }
}