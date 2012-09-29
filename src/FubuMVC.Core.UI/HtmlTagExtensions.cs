using System;
using System.Collections.Generic;
using System.Linq;
using FubuLocalization;
using HtmlTags;
using FubuCore;

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

        // TODO - get this into HtmlTAgs itself
        public static HtmlTag Name(this HtmlTag tag, string name)
        {
            return tag.Attr("name", name);
        }

        public static HtmlTag Value(this HtmlTag tag, string value)
        {
            return tag.Attr("value", value);
        }
    }
}