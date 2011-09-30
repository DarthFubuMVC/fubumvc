using System;
using System.IO;
using System.Net;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    // TODO -- move this to the new Http folder after ReST is applied
    public interface IHttpOutputWriter
    {
        void AppendHeader(string key, string value);
        void WriteFile(string file);
        void WriteContentType(string contentType);
        void Write(string content);
        void Redirect(string url);
        void WriteResponseCode(HttpStatusCode status);
        void AppendCookie(HttpCookie cookie);
    }

    public class NulloHttpOutputWriter : IHttpOutputWriter
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

        public void WriteResponseCode(HttpStatusCode status)
        {
        }

        public void AppendCookie(HttpCookie cookie)
        {
        }
    }

    // Warning -- untestable code ahead
    public class AspNetHttpOutputWriter : IHttpOutputWriter
    {
        private HttpResponse response
        {
            get { return HttpContext.Current.Response; }
        }

        public void AppendHeader(string key, string value)
        {
            response.AppendHeader(key, value);
        }

        public void WriteFile(string file)
        {
            response.WriteFile(file);
        }

        public void WriteContentType(string contentType)
        {
            response.ContentType = contentType;
        }

        public void Write(string content)
        {
            response.Write(content);
        }

        public void Redirect(string url)
        {
            response.Redirect(url, false);
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            response.StatusCode = (int) status;
        }

        public void AppendCookie(HttpCookie cookie)
        {
            response.AppendCookie(cookie);
        }
    }

    public static class HttpOutputWriterExtensions
    {
        // TODO -- add extensions for the MimeType class
    }
}