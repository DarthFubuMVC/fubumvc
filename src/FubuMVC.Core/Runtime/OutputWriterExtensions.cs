using System.Net;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
{
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

        public static void Write(this IOutputWriter writer, MimeType mimeType, string contents)
        {
            writer.Write(mimeType.Value, contents);
        }

        public static void WriteFile(this IOutputWriter writer, MimeType contentType, string localFilePath, string displayName)
        {
            writer.WriteFile(contentType.Value, localFilePath, displayName);
        }

        public static void AppendHeader(this IOutputWriter writer, HttpResponseHeader header, string value)
        {
            writer.AppendHeader(HttpResponseHeaders.HeaderNameFor(header), value);
        }

        public static void ContentType(this IOutputWriter writer, MimeType mimeType)
        {
            writer.AppendHeader(HttpResponseHeader.ContentType, mimeType.Value);
        }
    }
}