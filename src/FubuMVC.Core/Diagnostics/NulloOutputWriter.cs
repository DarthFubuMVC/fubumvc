using System;
using System.Net;
using System.Web;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public class NulloOutputWriter : IOutputWriter
    {
        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            
        }

        public void Write(string contentType, string renderedOutput)
        {
        }

        public void RedirectToUrl(string url)
        {
        }

        public void AppendCookie(HttpCookie cookie)
        {
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
        }

        public RecordedOutput Record(Action action)
        {
            return new RecordedOutput("", "");
        }
    }
}