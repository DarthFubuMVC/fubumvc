using System.Net;
using System.Web;

namespace FubuMVC.Core.Http
{
    // TODO -- move this to the new Http folder after ReST is applied
    public interface IHttpWriter
    {
        void AppendHeader(string key, string value);
        void WriteFile(string file);
        void WriteContentType(string contentType);
        void Write(string content);
        void Redirect(string url);
        void WriteResponseCode(HttpStatusCode status);
        void AppendCookie(HttpCookie cookie);
    }

    // Warning -- untestable code ahead
}