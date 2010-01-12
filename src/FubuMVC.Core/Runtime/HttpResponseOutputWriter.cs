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
                response.AppendHeader("content-disposition", "attachment; filename=" + displayName);
            }

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