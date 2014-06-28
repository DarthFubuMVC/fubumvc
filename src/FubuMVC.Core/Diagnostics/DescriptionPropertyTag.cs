using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{
    public class DescriptionPropertyTag : HtmlTag
    {
        public DescriptionPropertyTag(string key, string value)
            : base("div")
        {
            AddClass("desc-prop");
            Add("b").Text(key);
            Add("span").Text(value);
        }
    }
}