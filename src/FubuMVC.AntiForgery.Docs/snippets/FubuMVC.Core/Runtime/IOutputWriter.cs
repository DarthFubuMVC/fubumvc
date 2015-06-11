using System;
using System.IO;
using System.Net;
using FubuMVC.Core.Caching;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Core.Runtime
{
    // SAMPLE: ioutputwriter
    /// <summary>
    /// Primary abstraction to write to the Http response
    /// </summary>
    public interface IOutputWriter
    {
        /// <summary>
        /// Write the contents of a file to the Http response body with the Content-Type header and display name header
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="localFilePath"></param>
        /// <param name="displayName"></param>
        void WriteFile(string contentType, string localFilePath, string displayName);

        /// <summary>
        /// Write string content to the response body with the designated contentType mimetype (e.g., "text/html")
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="renderedOutput"></param>
        void Write(string contentType, string renderedOutput);

        /// <summary>
        /// Writes string content to the response body
        /// </summary>
        /// <param name="renderedOutput"></param>
        void Write(string renderedOutput);

        /// <summary>
        /// Writes a 302 redirect to the url to the request 
        /// </summary>
        /// <param name="url"></param>
        void RedirectToUrl(string url);

        /// <summary>
        /// Appends a Set-Cookie header to the response
        /// </summary>
        /// <param name="cookie"></param>
        void AppendCookie(Cookie cookie);

        /// <summary>
        /// Appends the named header value to the response
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AppendHeader(string key, string value);

        /// <summary>
        /// Writes to the response body as a stream
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="output"></param>
        void Write(string contentType, Action<Stream> output);

        /// <summary>
        /// Writes a response code back to the response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="description"></param>
        void WriteResponseCode(HttpStatusCode status, string description = null);

        /// <summary>
        /// Sets the IOutputWriter to "record" mode in caching scenarios
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IRecordedOutput Record(Action action);

        /// <summary>
        /// Replays all recorded output
        /// </summary>
        /// <param name="output"></param>
        void Replay(IRecordedOutput output);


        /// <summary>
        /// Flushes all output to the Http response
        /// </summary>
        void Flush();
    }
    // ENDSAMPLE

   
}