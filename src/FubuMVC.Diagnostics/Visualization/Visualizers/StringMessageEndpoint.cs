using FubuCore.Logging;
using HtmlTags;

namespace FubuMVC.Diagnostics.Visualization.Visualizers
{
    public class StringMessageEndpoint
    {
        public HtmlTag VisualizePartial(StringMessage message)
        {
            return new CommentTag(message.Message);
        }
    }
}