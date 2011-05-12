using System;
using System.Net;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public interface IOutputWriter
    {
        void WriteFile(string contentType, string localFilePath, string displayName);
        void Write(string contentType, string renderedOutput);
        void RedirectToUrl(string url);
        void AppendCookie(HttpCookie cookie);

        void WriteResponseCode(HttpStatusCode status);
        RecordedOutput Record(Action action);
    }



    public static class OutputWriterExtensions
    {
        public static void WriteHtml(this IOutputWriter writer, string content)
        {
            writer.Write(MimeType.Html.ToString(), content);
        }

        public static void WriteHtml(this IOutputWriter writer, object content)
        {
            writer.Write(MimeType.Html.ToString(), content == null ? string.Empty : content.ToString());
        }
    }
}