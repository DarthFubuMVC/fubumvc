using FubuCore.Descriptions;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{
    public class ChildDescriptionTag : HtmlTag
    {
        public ChildDescriptionTag(string name, Description child)
            : base("div")
        {
            AddClass("desc-child");

            Add("div", title =>
            {
                title.AddClass("desc-child-title");
                title.Add("b").Text(name);
                title.Add("i").Text(child.Title);
            });

            Add("div").AddClass("desc-child-body").Append(new DescriptionBodyTag(child));
        }
    }
}