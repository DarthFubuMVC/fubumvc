using System;
using System.Collections.Generic;
using System.Linq;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.UI
{
    public static class HtmlTagExtensions
    {

        // TODO -- get almost all of this into HtmlTAgs
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


        // TODO - get this into HtmlTAgs itself
        public static HtmlTag Name(this HtmlTag tag, string name)
        {
            return tag.Attr("name", name);
        }

        // TODO - get this into HtmlTAgs itself
        public static HtmlTag Value(this HtmlTag tag, string value)
        {
            return tag.Attr("value", value);
        }

        public static string Mustached(this string key)
        {
            return "{{" + key + "}}";
        }

        public static HtmlTag MustacheAttr(this HtmlTag tag, string attributeName, string key)
        {
            return tag.Attr(attributeName, key.Mustached());
        }

        public static HtmlTag MustacheValue(this HtmlTag tag, string key)
        {
            return tag.Value(key.Mustached());
        }

        public static HtmlTag MustacheText(this HtmlTag tag, string key)
        {
            return tag.Text(key.Mustached());
        }
    }
}