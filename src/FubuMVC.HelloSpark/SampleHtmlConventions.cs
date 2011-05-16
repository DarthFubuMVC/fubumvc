using FubuMVC.Core.UI;

namespace FubuMVC.HelloSpark
{
    public class SampleHtmlConventions : HtmlConventionRegistry
    {
        public SampleHtmlConventions()
        {
            Editors.IfPropertyIs<int>().Modify(tag => tag.AddClass("number"));
        }
    }
}