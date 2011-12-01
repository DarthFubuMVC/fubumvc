using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace Serenity.Endpoints
{
    public class EndpointDriver
    {
        private readonly IUrlRegistry _urls;

        public EndpointDriver(IUrlRegistry urls)
        {
            _urls = urls;
        }

        public HttpResponse Send(EndpointInvocation invocation)
        {
            var request = requestForUrlTarget(invocation.Target);
            request.Method = invocation.Method;
            request.ContentType = invocation.ContentType;
            request.As<HttpWebRequest>().Accept = invocation.Accept;

            request.WriteText(invocation.GetContent());

            return request.ToHttpCall();
        } 

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

        public HttpResponse PostXml<T>(T target, string contentType = "text/xml", string accept = "*/*")
        {
            return post(target, contentType, accept, stream =>
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, target);
            });
        }

        public HttpResponse GetHtml(object subject)
        {
            var request = requestForUrlTarget(subject);
            request.Method = "GET";
            request.ContentType = MimeType.HttpFormMimetype;


            return request.ToHttpCall();
        }

        public HttpResponse GetHtml(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = MimeType.HttpFormMimetype;


            return request.ToHttpCall();
        }

        public string ReadTextFrom(object input)
        {
            var url = _urls.UrlFor(input);
            return new WebClient().DownloadString(url);
        }



        private HttpResponse post(object urlTarget, string contentType, string accept, Action<Stream> setRequest)
        {
            WebRequest request = requestForUrlTarget(urlTarget);
            request.ContentType = contentType;

            request.Method = "POST";

            request.As<HttpWebRequest>().Accept = accept;

            var stream = request.GetRequestStream();
            setRequest(stream);
            stream.Close();

            return request.ToHttpCall();
        }

        private WebRequest requestForUrlTarget(object urlTarget)
        {
            var url = _urls.UrlFor(urlTarget);
            return WebRequest.Create(url);
        }


    }
}