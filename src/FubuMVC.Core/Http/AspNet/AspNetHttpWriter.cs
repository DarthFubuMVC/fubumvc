using System.Net;
using System.Web;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetHttpWriter : IHttpWriter
    {
        private readonly HttpResponseBase _response;


        public AspNetHttpWriter(HttpResponseBase response)
        {
            _response = response;
        }

        public void AppendHeader(string key, string value)
        {
            _response.AppendHeader(key, value);
        }

        public void WriteFile(string file)
        {
            _response.WriteFile(file);
        }

        public void WriteContentType(string contentType)
        {
            _response.ContentType = contentType;
        }

        public void Write(string content)
        {
            _response.Write(content);
        }

        public void Redirect(string url)
        {
            _response.Redirect(url, false);
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            _response.StatusCode = (int) status;
        }

        public void AppendCookie(HttpCookie cookie)
        {
            _response.AppendCookie(cookie);
        }
    }
}