using System.Net;
using System.Web;

namespace FubuMVC.Core.Http
{
    public class NulloHttpWriter : IHttpWriter
    {
        public void AppendHeader(string key, string value)
        {
            
        }

        public void WriteFile(string file)
        {
        }

        public void WriteContentType(string contentType)
        {
        }

        public void Write(string content)
        {
        }

        public void Redirect(string url)
        {
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
        }

        public void AppendCookie(HttpCookie cookie)
        {
        }
    }
}