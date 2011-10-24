using System;
using System.Net;
using System.Web;

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

        public string ApplicationRoot()
        {
            return _request.ApplicationPath.TrimEnd('/');
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