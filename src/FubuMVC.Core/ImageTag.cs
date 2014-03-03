using HtmlTags;

namespace FubuMVC.Core
{
    // TODO -- move this to HtmlTags!!!!!
    public class ImageTag : HtmlTag
    {
        public ImageTag(string url)
            : base("img")
        {
            Attr("src", url);
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