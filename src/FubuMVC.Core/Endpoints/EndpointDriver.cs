using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using System.Linq;

namespace FubuMVC.Core.Endpoints
{
    public enum ContentFormat
    {
        httpForm,
        json,
        xml
    }

    /// <summary>
    /// Primarily used to "drive" a remote or embedded FubuMVC application in testing scenarios
    /// by issuing http requests and reading the corresponding response
    /// </summary>
    public class EndpointDriver
    {
        private readonly IUrlRegistry _urls;
        private readonly string _baseUrl;

        public EndpointDriver(IUrlRegistry urls, string baseUrl = null)
        {
            _urls = urls;
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Send an http request to the application
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        public HttpResponse Send(EndpointInvocation invocation)
        {
            var request = requestForUrlTarget(invocation.Target);
            request.Method = invocation.Method;
            request.ContentType = invocation.ContentType;
            request.As<HttpWebRequest>().Accept = invocation.Accept;

            request.WriteText(invocation.GetContent());

            return request.ToHttpCall();
        } 

        /// <summary>
        /// Reads all the public gettable properties of the target and issues an Http form post against the endpoint that
        /// accepts a POST of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="contentType"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public HttpResponse PostAsForm<T>(T target, string contentType = "application/x-www-form-urlencoded", string accept="*/*")
        {
            var dictionary = new Dictionary<string, object>();
            new TypeDescriptorCache().ForEachProperty(typeof(T), prop =>
            {
                var rawValue = prop.GetValue(target, null);
                var httpValue = rawValue == null ? string.Empty : rawValue.ToString().UrlEncoded();

                dictionary.Add(prop.Name, httpValue);
            });

            var post = dictionary.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)).Join("&");

            return this.post(target, contentType, accept, stream =>
            {
                var bytes = Encoding.Default.GetBytes(post);

                stream.Write(bytes, 0, bytes.Length);
            });
        }

        /// <summary>
        /// Posts a file to the endpoint that accepts type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="fileInputFormName"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public HttpResponse PostFile<T>(T target, string fileInputFormName, string accept="*/*")
        {
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var wr = (HttpWebRequest) requestForUrlTarget(target, "POST");
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Accept = "*/*";
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            using (var rs = wr.GetRequestStream())
            {
                const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                var formitem = string.Format(formdataTemplate, "foo", "bar");
                var formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                const string headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                var header = string.Format(headerTemplate, fileInputFormName, "test.pdf", "application/pdf");
                var headerbytes = Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                var fileBytes = Encoding.UTF8.GetBytes("test.pdf contents");
                rs.Write(fileBytes, 0, fileBytes.Length);

                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
            }

            return wr.ToHttpCall();
        }

        /// <summary>
        /// Posts target as serialized Json to the endpoint that accepts a POST of input.GetType()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="input"></param>
        /// <param name="contentType"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public HttpResponse PostJson<T>(T target, object input, string contentType = "text/json", string accept = "*/*")
        {
            return post(target, contentType, accept, stream =>
            {
                var serializer = new JavaScriptSerializer();
                

                var json = serializer.Serialize(input);
                var bytes = Encoding.Default.GetBytes(json);
                
                stream.Write(bytes, 0, bytes.Length);
            });
        }

        /// <summary>
        /// Posts target as serialized Json to the endpoint that accepts type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="contentType"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public HttpResponse PostJson<T>(T target, string contentType = "text/json", string accept = "*/*")
        {
            return post(target, contentType, accept, stream =>
            {
                var serializer = new JavaScriptSerializer();
                

                var json = serializer.Serialize(target);
                var bytes = Encoding.Default.GetBytes(json);
                
                stream.Write(bytes, 0, bytes.Length);
            });
        }

        /// <summary>
        /// Posts target as serialized xml to the endpoint that accepts type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="contentType"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public HttpResponse PostXml<T>(T target, string contentType = "text/xml", string accept = "*/*")
        {
            return post(target, contentType, accept, stream =>
            {
                var writer = new StringWriter();

                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, target);

                var bytes = Encoding.Default.GetBytes(writer.ToString());

                stream.Write(bytes, 0, bytes.Length);
            });
        }

        /// <summary>
        /// Issues an Http GET for the url represented by subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="acceptType"></param>
        /// <returns></returns>
        public HttpResponse GetBySubject(object subject, string acceptType = "*/*")
        {
            var request = requestForUrlTarget(subject);
            request.Method = "GET";
            request.ContentType = MimeType.HttpFormMimetype;

            request.As<HttpWebRequest>().Accept = acceptType;

            return request.ToHttpCall();
        }

        /// <summary>
        /// Issues an Http GET to the designated controller action endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <param name="acceptType"></param>
        /// <returns></returns>
        public HttpResponse Get<T>(Expression<Action<T>> expression, string categoryOrHttpMethod = null, string acceptType = "*/*")
        {
            var url = _urls.UrlFor(expression, categoryOrHttpMethod).ToAbsoluteUrl(_baseUrl);
            return Get(url, acceptType);
        }

        public HttpResponse GetByInput<T>(T model, string categoryOrHttpMethod = "GET", string acceptType = "*/*", Action<HttpWebRequest> configure = null, string acceptEncoding = null)
        {
            var url = _urls.UrlFor(model, categoryOrHttpMethod).ToAbsoluteUrl(_baseUrl);
            return Get(url, acceptType, configure:configure, acceptEncoding:acceptEncoding);
        }

        /// <summary>
        /// Executes a GET to the designated url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="etag"></param>
        /// <param name="configure"> </param>
        /// <returns></returns>
        public HttpResponse Get(string url, string acceptType = "*/*", string etag = null, Action<HttpWebRequest> configure = null, string acceptEncoding = null, DateTime? ifModifiedSince = null)
        {
            url = url.ToAbsoluteUrl(_baseUrl);

            var request = (HttpWebRequest) WebRequest.Create(url.ToAbsoluteUrl());
            request.Method = "GET";
            request.ContentType = MimeType.HttpFormMimetype;
            request.UserAgent = "EndpointDriver User Agent 1.0";
            request.As<HttpWebRequest>().Accept = acceptType;
            request.As<HttpWebRequest>().CookieContainer = new CookieContainer();

            if(configure != null)
            {
                configure(request);
            }

            if (ifModifiedSince.HasValue)
            {
                request.As<HttpWebRequest>().IfModifiedSince = ifModifiedSince.Value;
            }

            if (etag.IsNotEmpty())
            {
                request.Headers[HttpRequestHeader.IfNoneMatch] = etag;
            }

            if (acceptEncoding.IsNotEmpty())
            {
                request.Headers[HttpRequestHeader.AcceptEncoding] = acceptEncoding;
            }

            return request.ToHttpCall();
        }

        public HttpResponse Head(string url)
        {
            url = url.ToAbsoluteUrl(_baseUrl);

            var request = (HttpWebRequest)WebRequest.Create(url.ToAbsoluteUrl());
            request.Method = "HEAD";

            return request.ToHttpCall();
        }

        /// <summary>
        /// Issues an Http GET to the endpoint that accepts input.GetType() and reads the 
        /// response body as text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ReadTextFrom(object input)
        {
            var url = _urls.UrlFor(input).ToAbsoluteUrl(_baseUrl);
            return new WebClient().DownloadString(url);
        }

        /// <summary>
        /// Issues an Http GET to the endpoint that contains the designated controller
        /// action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string ReadTextFrom<T>(Expression<Action<T>> expression)
        {
            var url = _urls.UrlFor(expression);
            url = url.ToAbsoluteUrl(_baseUrl);

            return new WebClient().DownloadString(url);
        }



        private HttpResponse post(object urlTarget, string contentType, string accept, Action<Stream> setRequest)
        {
            WebRequest request = requestForUrlTarget(urlTarget, "POST");
            request.ContentType = contentType;

            request.Method = "POST";

            request.As<HttpWebRequest>().Accept = accept;
            request.As<HttpWebRequest>().CookieContainer = new CookieContainer();

            var stream = request.GetRequestStream();
            setRequest(stream);
            stream.Close();

            return request.ToHttpCall();
        }

        private WebRequest requestForUrlTarget(object urlTarget, string categoryOrHttpMethod = null)
        {
            var url = _urls.UrlFor(urlTarget, categoryOrHttpMethod);
            url = url.ToAbsoluteUrl(_baseUrl);

            return WebRequest.Create(url);
        }


    }
}