using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
{
    public static class OutputWriterExtensions
    {
        public static Task WriteHtml(this IOutputWriter writer, string content)
        {
            return writer.Write(MimeType.Html.ToString(), content);
        }

        public static Task WriteHtml(this IOutputWriter writer, object content)
        {
            return writer.Write(MimeType.Html.ToString(), content?.ToString() ?? string.Empty);
        }

        public static Task Write(this IOutputWriter writer, MimeType mimeType, string contents)
        {
            return writer.Write(mimeType.Value, contents);
        }

        // TODO -- this probably needs to be async
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