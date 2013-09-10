using System;
using System.IO;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http.Compression;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetHttpWriter : IHttpWriter
    {
        private readonly HttpResponseBase _response;


        public AspNetHttpWriter(HttpResponseBase response)
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