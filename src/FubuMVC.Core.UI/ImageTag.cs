using FubuLocalization;
using HtmlTags;

namespace FubuHtml
{
    public class ImageTag : HtmlTag
    {
        public ImageTag(string url)
            : base("img")
        {
            Attr("src", url);
        }

        public ImageTag AlternateText(StringToken token)
        {
            Attr("alt", token.ToString());
            return this;
        }

        public ImageTag Width(int width)
        {
            Style("width", width + "px");
            return this;
        }

        public ImageTag Height(int height)
        {
            Style("height", height + "px");
            return this;
        }
    }
}