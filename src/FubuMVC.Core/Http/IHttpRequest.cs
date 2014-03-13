using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using FubuCore.Util;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Core.Http
{
    public interface IHttpRequest
    {
        /// <summary>
        ///   Full url of the request, never contains a trailing /
        /// </summary>
        /// <returns></returns>
        string RawUrl();

        /// <summary>
        ///   Url relative to the application
        /// </summary>
        /// <returns></returns>
        string RelativeUrl();

        /// <summary>
        ///  Gets the full URI (with schema, server, port, etc.) as requested by the client
        /// </summary>
        /// <returns>Fully qualified URI as requested by the client</returns>
        string FullUrl();

        /// <summary>
        /// Converts a relative URL into an app-qualified URL
        /// </summary>
        /// <param name="url">an absolute, relative, or app-relative URL</param>
        /// <returns>an app-qualified URL</returns>
        /// <remarks>This method does not return a fully qualified URI. That is, the return value will not contain the scheme (e.g. http, https), the server host or IP, or port number.</remarks>
        string ToFullUrl(string url);

        /// <summary>
        /// Name of the Http Method, i.e., POST/GET/PUT/DELETE
        /// </summary>
        /// <returns></returns>
        string HttpMethod();

        /// <summary>
        /// Is there any value(s) for the header key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool HasHeader(string key);

        /// <summary>
        /// Get all the header values for a given header key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<string> GetHeader(string key);

        /// <summary>
        /// All request header keys
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> AllHeaderKeys();


        /// <summary>
        /// Exposes the query string keys and values for
        /// the current Http request
        /// </summary>
        NameValueCollection QueryString { get; }

        /// <summary>
        /// Exposes the name/value form post data for 
        /// the current HTTP request if it exists
        /// </summary>
        NameValueCollection Form { get; }

        /// <summary>
        /// Read the body of the HTTP request as a Stream
        /// Note: you cannot reading the Stream and the Form
        /// is mutually exclusive within one request
        /// </summary>
        Stream Input { get; }

        /// <summary>
        /// Is the client still connected to this request?
        /// </summary>
        /// <returns></returns>
        bool IsClientConnected();

        /// <summary>
        /// Access to the Request Cookies
        /// </summary>
        ICookies Cookies { get; }
    }

}