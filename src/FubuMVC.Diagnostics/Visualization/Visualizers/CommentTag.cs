using FubuMVC.Core.UI.Bootstrap.Tags;
using HtmlTags;

namespace FubuMVC.Diagnostics.Visualization.Visualizers
{
    public class CommentTag : HtmlTag
    {
        public CommentTag(string text) : base("div")
        {
            AddClass("comment");
            this.PrependGlyph("icon-comment");
            Add("span").Text(text);
        }
    }
}