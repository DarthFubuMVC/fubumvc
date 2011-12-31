using FubuMVC.Core.UI;

namespace FubuMVC.HelloRazor
{
    public class SampleHtmlConventions : HtmlConventionRegistry
    {
        public SampleHtmlConventions()
        {
            Editors.IfPropertyIs<int>().Modify(tag => tag.AddClass("number"));
        }
    }
}