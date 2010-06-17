using System.IO;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public class HttpResponseOutputWriter : IOutputWriter
    {
        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.ContentType = contentType;

			if (displayName != null)
			{
				response.AppendHeader("Content-Disposition", "attachment; filename=" + displayName);
			}

			var fileInfo = new FileInfo(localFilePath);
			response.AppendHeader("Content-Length", fileInfo.Length.ToString());

            response.WriteFile(localFilePath);
        }

        public void Write(string contentType, string renderedOutput)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.ContentType = contentType;
            response.Write(renderedOutput);
        }

        public void RedirectToUrl(string url)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Redirect(url, false);
        }
    }
}