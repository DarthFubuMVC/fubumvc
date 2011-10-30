using System;
using System.IO;
using System.Net;
using System.Web;
using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Runtime
{
    public interface IOutputWriter
    {
        void WriteFile(string contentType, string localFilePath, string displayName);
        void Write(string contentType, string renderedOutput);
        void RedirectToUrl(string url);
        void AppendCookie(HttpCookie cookie);

        void AppendHeader(string key, string value);

        void Write(string contentType, Action<Stream> output);

        void WriteResponseCode(HttpStatusCode status);
        IRecordedOutput Record(Action action);
        void Replay(IRecordedOutput output);
    }

   
}