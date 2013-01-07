using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;

namespace FubuMVC.Core.Endpoints
{
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

        public CookieCollection Cookies
        {
            get { return _response.Cookies; }
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

        public long ContentLength()
        {
            return _response.ContentLength;
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
            if (_body.Contains("Error")) return null;

            var document = new XmlDocument();
            document.LoadXml(_body);

            return document;
        }

        public override string ToString()
        {

            return _body;
        }

        public string Source()
        {
            return _body;
        }

        /// <summary>
        /// Tries to deserialize the response body as Json using the
        /// JavaScriptSerializer to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadAsJson<T>()
        {
            return new JavaScriptSerializer().Deserialize<T>(_body);
        }

        /// <summary>
        /// Reads the response body as a string
        /// </summary>
        /// <returns></returns>
        public string ReadAsText()
        {
            return ToString();
        }

        /// <summary>
        /// Returns the value of the etag header in the response
        /// </summary>
        /// <returns></returns>
        public string Etag()
        {
            return _response.Headers[HttpResponseHeader.ETag];
        }

        /// <summary>
        /// Tries to read the response body into an XmlDocument
        /// </summary>
        /// <returns></returns>
        public XmlDocument ReadAsXml()
        {
            var document = new XmlDocument();
            document.LoadXml(ToString());

            return document;
        }
    }
}