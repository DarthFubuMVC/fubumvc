using System.Linq;
using FubuCore;
using FubuLocalization;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class HtmlTagExtensions
    {


        public static HtmlTag Text(this HtmlTag tag, StringToken token)
        {
            return tag.Text(token == null ? string.Empty : token.ToString());
        }

        public static HtmlTag Attr(this HtmlTag tag, string attName, StringToken token)
        {
            return tag.Attr(attName, token.ToString());
        }


        public static HtmlTag ForChild(this ITagSource parent, string tagName)
        {
            return parent.AllTags().First(child => child.TagName().EqualsIgnoreCase(tagName));
        }
    }
}