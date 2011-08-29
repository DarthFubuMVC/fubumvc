using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Linq;

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

    public interface IResponseCaching
    {
        void CacheRequestAgainstFileChanges(IEnumerable<string> localFiles);
    }

    public class ResponseCaching : IResponseCaching
    {
        private readonly HttpCachePolicy _cache;
        private readonly HttpResponse _response;

        public ResponseCaching()
        {
            _response = HttpContext.Current.Response;
            _cache = _response.Cache;
        }

        public void CacheRequestAgainstFileChanges(IEnumerable<string> localFiles)
        {
            _response.AddFileDependencies(localFiles.ToArray());

            _cache.VaryByParams["files"] = true;
            _cache.SetLastModifiedFromFileDependencies();
            _cache.SetETagFromFileDependencies();
            _cache.SetCacheability(HttpCacheability.Public);
        }
    }
}