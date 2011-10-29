using System;
using System.IO;
using System.Net;
using System.Web;

namespace FubuMVC.Core.Http
{
    public interface IHttpWriter
    {
        void AppendHeader(string key, string value);
        void WriteFile(string file);
        void WriteContentType(string contentType);
        void Write(string content);
        void Redirect(string url);
        void WriteResponseCode(HttpStatusCode status);
        void AppendCookie(HttpCookie cookie);

        void Write(Action<Stream> output);
    }

}