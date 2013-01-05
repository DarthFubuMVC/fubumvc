using System;
using System.IO;
using System.Net;
using FubuMVC.Core.Caching;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Core.Runtime
{
    public interface IOutputWriter : IDisposable
    {
        void WriteFile(string contentType, string localFilePath, string displayName);
        void Write(string contentType, string renderedOutput);

        void Write(string renderedOutput);


        void RedirectToUrl(string url);
        void AppendCookie(Cookie cookie);

        void AppendHeader(string key, string value);

        void Write(string contentType, Action<Stream> output);

        void WriteResponseCode(HttpStatusCode status, string description = null);
        IRecordedOutput Record(Action action);
        void Replay(IRecordedOutput output);


        /// <summary>
        /// Flushes all output to the Http response
        /// </summary>
        void Flush();
    }

   
}