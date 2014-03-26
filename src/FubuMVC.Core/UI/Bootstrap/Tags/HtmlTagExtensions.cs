using HtmlTags;

namespace FubuMVC.Core.UI.Bootstrap.Tags
{
    public static class HtmlTagExtensions
    {
        public static HtmlTag PrependAnchor(this HtmlTag tag)
        {
            var a = new HtmlTag("a").Attr("href", "#" + tag.Id());
            a.Next = tag;

            return a;
        }

        public static HtmlTag PrependGlyph(this HtmlTag tag, string glyphName)
        {
            var glyph = new HtmlTag("i").AddClass(glyphName);
            var literal = new LiteralTag(" ");

            tag.InsertFirst(literal);
            tag.InsertFirst(glyph);

            return tag;
        }
    }
}