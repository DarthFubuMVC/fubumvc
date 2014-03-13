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

    public class StandInHttpRequest : IHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string ApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public StandInHttpRequest()
        {
            Input = new MemoryStream();
            QueryString = new NameValueCollection();
        }

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string FullUrl()
        {
            return StubFullUrl;
        }

        public string ToFullUrl(string url)
        {
	        return url.ToAbsoluteUrl();
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }

        public readonly Cache<string, IList<string>> HeaderValues = new Cache<string, IList<string>>(name => new List<string>()); 

        public bool HasHeader(string key)
        {
            return HeaderValues.Has(key);
        }

        public IEnumerable<string> GetHeader(string key)
        {
            return HeaderValues[key];
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return HeaderValues.GetAllKeys();
        }

        public NameValueCollection QueryString { get; private set; }
        public NameValueCollection Form { get; private set; }
        public Stream Input { get; private set; }

        public void AppendHeader(string key, string value)
        {
            HeaderValues[key].Add(value);
        }

        public bool IsClientConnected()
        {
            return true;
        }

        public ICookies Cookies
        {
            get
            {
                return new Cookies.Cookies(this);
            }
        }
    }
}