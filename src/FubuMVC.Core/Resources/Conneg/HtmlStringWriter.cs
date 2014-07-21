using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class HtmlStringWriter<T> : IMediaWriter<T>, DescribesItself
    {
        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            context.Writer.WriteHtml(resource.ToString());
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }

        public void Describe(Description description)
        {
            description.Title = "Write output model ToString() as text/html";
            description.ShortDescription = null;
        }
    }
}