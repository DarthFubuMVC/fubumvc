using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class StringWriter : IMediaWriter<string>
    {
        public void Write(string mimeType, IFubuRequestContext context, string resource)
        {
            context.Writer.Write(mimeType, resource);
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.Text.Value;
                yield return MimeType.Html.Value;
            }
        }
    }
}