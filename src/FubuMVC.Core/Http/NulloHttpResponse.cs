using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Http
{
    public class NulloHttpResponse : IHttpResponse
    {
        public void AppendHeader(string key, string value)
        {
            
        }

        public void WriteFile(string file)
        {
        }

        public void WriteContentType(string contentType)
        {
        }

        public void Write(string content)
        {
        }

        public void Redirect(string url)
        {
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
        }

        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public IEnumerable<string> HeaderValueFor(string headerKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Header> AllHeaders()
        {
            throw new NotImplementedException();
        }

        public void UseEncoding(IHttpContentEncoding encoding)
        {
        }

        public void Write(Action<Stream> output)
        {
        }

        public void Flush()
        {
        }
    }
}