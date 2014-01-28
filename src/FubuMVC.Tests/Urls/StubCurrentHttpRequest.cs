using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Http;

namespace FubuMVC.Tests.Urls
{

    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public StubCurrentHttpRequest()
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
    }
}