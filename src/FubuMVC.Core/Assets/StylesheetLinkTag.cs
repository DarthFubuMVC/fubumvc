using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class StylesheetLinkTag : HtmlTag
    {
        public StylesheetLinkTag(string url)
            : base("link")
        {
            Attr("href", url);
            Attr("rel", "stylesheet");
            Attr("type", MimeType.Css.Value);
        }
    }
}