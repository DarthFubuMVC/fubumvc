using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Urls;
using System.Linq;

namespace Serenity
{
    public enum EndpointFormatting
    {
        json,
        xml,
        form,
        other
    }

    public class EndpointInvocation
    {
        public string Accept = "*/*";
        public string ContentType;
        public object Target;
        public EndpointFormatting SendAs = EndpointFormatting.json;
        public string Content;
        public string Method = "POST";

        public string GetContent()
        {
            switch (SendAs)
            {
                case EndpointFormatting.json:
                    return writeJson();

                case EndpointFormatting.xml:
                    return writeXml();

                case EndpointFormatting.other:
                    return Content;

                case EndpointFormatting.form:
                    return writeForm();
            }

            return null;
        }

        private string writeForm()
        {
            var document = new XmlDocument();
            document.LoadXml(writeXml());

            var list = new List<string>();

            foreach (XmlElement element in document.DocumentElement.ChildNodes)
            {
                var data = "{0}={1}".ToFormat(element.Name, element.InnerText.UrlEncode());
                list.Add(data);
            }

            return list.Join("&");
        }


        private string writeXml()
        {
            var serializer = new XmlSerializer(Target.GetType());
            var builder = new StringBuilder();
            var writer = new XmlTextWriter(new StringWriter(builder));
            serializer.Serialize(writer, Target);

            return builder.ToString();
        }

        private string writeJson()
        {
            return new JavaScriptSerializer().Serialize(Target);
        }
    }


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
                var json = new JavaScriptSerializer().Serialize(target);
                var writer = new StreamWriter(stream);

                writer.Write(json);
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
            request.ContentType = CurrentMimeType.HttpFormMimetype;


            return request.ToHttpCall();
        }



        private HttpResponse post(object urlTarget, string contentType, string accept, Action<Stream> setRequest)
        {
            WebRequest request = requestForUrlTarget(urlTarget);
            request.ContentType = contentType;

            request.Method = "POST";
            request.Headers[HttpRequestHeader.Accept] = accept;

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
            var result = request.BeginGetResponse(r => { }, null);
            try
            {
                var response = request.EndGetResponse(result).As<HttpWebResponse>();
                return new HttpResponse(response);
            }
            catch (WebException e)
            {
                return new HttpResponse(e.Response.As<HttpWebResponse>());
            }
        }

        public static void WriteText(this WebRequest request, string content)
        {
            request.ContentLength = content.Length;
            var stream = request.GetRequestStream();

            var array = Encoding.Default.GetBytes(content);
            stream.Write(array, 0, array.Length);
            stream.Close();
        }
    }

    public class HttpResponse
    {
        private readonly HttpWebResponse _response;
        private readonly string _body;

        public HttpResponse(HttpWebResponse response)
        {
            _response = response;
            _body = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
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
                var response = ResponseHeaderFor(HttpResponseHeader.ContentType);
                if (response.Contains(";"))
                {
                    return response.Split(';').First();
                }

                return response;
            }
        }

        public XmlDocument ToXml()
        {
            var document = new XmlDocument();
            document.Load(_body);

            return document;
        }

        public override string ToString()
        {

            return _body;
        }
    }
}