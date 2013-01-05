using System;
using System.IO;
using System.Net;
using System.Web;
using FubuMVC.Core.Http.Compression;

namespace FubuMVC.Core.Http
{
    public class NulloHttpWriter : IHttpWriter
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