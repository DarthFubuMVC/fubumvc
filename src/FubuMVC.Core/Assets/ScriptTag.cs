using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class ScriptTag : HtmlTag
    {
        public ScriptTag(string url)
            : base("script")
        {
            // http://stackoverflow.com/a/1288319/75194 
            Attr("type", "text/javascript");
            Attr("src", url);
        }
    }
}