using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics.Visualization
{
    public class DescriptionWriter : IMediaWriter<Description>, DescribesItself
    {
        public void Write(string mimeType, IFubuRequestContext context, Description resource)
        {
            var tag = context.Service<IVisualizer>().VisualizeDescription(resource);
            context.Writer.WriteHtml(tag);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "Invokes the IVisualizer interface to visualize the Description";
        }
    }
}