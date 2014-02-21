using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetHttpResponse : IHttpResponse
    {
        private readonly HttpResponseBase _response;


        public AspNetHttpResponse(HttpResponseBase response)
        {
            _response = response;
        }

        public void AppendHeader(string key, string value)
        {
            _response.AppendHeader(key, value);
        }

        public void WriteFile(string file)
        {
            _response.WriteFile(file);
        }

        public void WriteContentType(string contentType)
        {
            _response.ContentType = contentType;
        }

        public void Write(string content)
        {
            _response.Write(content);
        }

        public void Redirect(string url)
        {
            _response.Redirect(url, false);
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            _response.StatusCode = (int) status;
            if (description.IsNotEmpty()) _response.StatusDescription = description;
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public string StatusDescription
        {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public IEnumerable<string> HeaderValueFor(HttpResponseHeader key)
        {
            return HeaderValueFor(HttpResponseHeaders.HeaderNameFor(key));
        }

        public IEnumerable<string> HeaderValueFor(string headerKey)
        {
            return new []{_response.Headers[headerKey]};
        }

        public IEnumerable<Header> AllHeaders()
        {
            var keys = _response.Headers.AllKeys;
            return keys.Select(x => new Header(x, _response.Headers[x])).ToArray();
        }

        public void UseEncoding(IHttpContentEncoding encoding)
        {
            _response.Filter = encoding.Encode(_response.Filter);
        }

        public void Write(Action<Stream> output)
        {
            output(_response.OutputStream);
        }

        public void Flush()
        {
            if (_response.IsClientConnected && !_response.IsRequestBeingRedirected)
            {
                _response.Flush();
            }
        }
    }
}