using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Urls;

namespace Serenity
{
    public class EndpointDriver
    {
        private readonly IUrlRegistry _urls;

        public EndpointDriver(IUrlRegistry urls)
        {
            _urls = urls;
        }

        public HttpResponse PostJson<T>(T target, string contentType = "text/json")
        {
            return post(target, contentType, stream =>
            {
                var json = new JavaScriptSerializer().Serialize(target);
                var writer = new StreamWriter(stream);

                writer.Write(json);
            });
        }

        public HttpResponse PostXml<T>(T target, string contentType = "text/xml")
        {
            return post(target, contentType, stream =>
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, target);
            });
        }

        public HttpResponse GetHtml(object subject)
        {
            var request = requestForUrlTarget(subject);
            request.Method = "GET";
            request.ContentType = CurrentMimeType.HttpFormMimetype;


            return request.ToHttpCall();
        }

        private HttpResponse post(object urlTarget, string contentType, Action<Stream> setRequest)
        {
            WebRequest request = requestForUrlTarget(urlTarget);
            request.ContentType = contentType;

            request.Method = "POST";

            setRequest(request.GetRequestStream());

            return request.ToHttpCall();
        }

        private WebRequest requestForUrlTarget(object urlTarget)
        {
            var url = _urls.UrlFor(urlTarget);
            return WebRequest.Create(url);
        }
    }

    public static class WebRequestResponseExtensions
    {
        public static HttpResponse ToHttpCall(this WebRequest request)
        {
            var response = request.GetResponse().As<HttpWebResponse>();
            return new HttpResponse(response);
        }
    }

    public class HttpResponse
    {
        private readonly HttpWebResponse _response;

        public HttpResponse(HttpWebResponse response)
        {
            _response = response;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return _response.StatusCode;
            }
        }

        public string ResponseHeaderFor(string name)
        {
            return _response.GetResponseHeader(name);
        }

        public string ResponseHeaderFor(HttpResponseHeader header)
        {
            return _response.Headers[header];
        }

        public Stream Output()
        {
            return _response.GetResponseStream();
        }

        public string ContentType
        {
            get
            {
                return ResponseHeaderFor(HttpResponseHeader.ContentType);
            }
        }

        public XmlDocument ToXml()
        {
            var document = new XmlDocument();
            document.Load(Output());

            return document;
        }

        public override string ToString()
        {
            var reader = new StreamReader(Output());
            return reader.ReadToEnd();
        }
    }
}