using System;
using System.IO;
using System.Net;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public class HttpResponseOutputWriter : IOutputWriter
    {
        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            response.ContentType = contentType;

			if (displayName != null)
			{
				response.AppendHeader("Content-Disposition", "attachment; filename=\"" + displayName+"\"");
			}

			var fileInfo = new FileInfo(localFilePath);
			response.AppendHeader("Content-Length", fileInfo.Length.ToString());

            response.WriteFile(localFilePath);

        }

        private HttpResponse response
        {
            get { return HttpContext.Current.Response; }
        }

        public void Write(string contentType, string renderedOutput)
        {
            response.ContentType = contentType;
            response.Write(renderedOutput);
        }

        public void RedirectToUrl(string url)
        {
            response.Redirect(url, false);
        }

        public void AppendCookie(HttpCookie cookie)
        {
            response.AppendCookie(cookie);
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            response.StatusCode = (int)status;
        }
    }
}