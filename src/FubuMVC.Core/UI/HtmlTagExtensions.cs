using System;
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

        public static T TextIfEmpty<T>(this T tag, string defaultText) where T : HtmlTag
        {
            if (tag.TagName().EqualsIgnoreCase("input")) throw new InvalidOperationException("You are attempting to set the inner text on an INPUT tag. If you wanted a textarea, call MultilineMode() first.");
            if (tag.Text().IsEmpty())
            {
                tag.Text(defaultText);
            }

            return tag;
        }

        public static T TextIfEmpty<T>(this T tag, StringToken token) where T : HtmlTag
        {
            return tag.TextIfEmpty(token.ToString());
        }
    }
}