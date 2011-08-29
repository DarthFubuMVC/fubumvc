using System;
using System.Net;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public interface IOutputWriter
    {
        // TODO -- change the signature to use MimeType?  Or just use overrides
        void WriteFile(string contentType, string localFilePath, string displayName);
        void Write(string contentType, string renderedOutput);
        void RedirectToUrl(string url);
        void AppendCookie(HttpCookie cookie);


        void WriteResponseCode(HttpStatusCode status);
        RecordedOutput Record(Action action);
    }
}