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
    }
}