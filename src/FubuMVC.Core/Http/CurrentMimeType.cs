using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    [DebuggerDisplay("{debuggerDisplay()}")]
    public class CurrentMimeType
    {
        private MimeTypeList _acceptTypes;

        public CurrentMimeType()
        {
        }

        // If contenttype is null, use the url encoded thing
        public CurrentMimeType(string contentType, string acceptType)
        {
            contentType = contentType ?? MimeType.HttpFormMimetype;
            if (contentType.Contains(";"))
            {
                var parts = contentType.ToDelimitedArray(';');
                contentType = parts.First();

                if (parts.Last().Contains("charset"))
                {
                    Charset = parts.Last().Split('=').Last();
                }
            }

            ContentType = contentType;

            AcceptTypes = new MimeTypeList(acceptType);
        }

        public string ContentType { get; set; }

        public MimeTypeList AcceptTypes
        {
            get { return _acceptTypes ?? new MimeTypeList(MimeType.Any); }
            set { _acceptTypes = value; }
        }

        public string Charset { get; set; }

        public bool AcceptsHtml()
        {
            return AcceptTypes.Contains(MimeType.Html.ToString());
        }

        public bool AcceptsAny()
        {
            return AcceptTypes.DefaultIfEmpty("*/*").Contains("*/*");
        }

        public string SelectFirstMatching(IEnumerable<string> candidates)
        {
            var exact = candidates.FirstOrDefault(x => AcceptTypes.Contains(x));
            return exact ?? (AcceptsAny() ? candidates.FirstOrDefault() : null);
        }

        string debuggerDisplay()
        {
            return ContentType + (Charset == null ?
                "" : ";charset=" + Charset);
        }
    }
}