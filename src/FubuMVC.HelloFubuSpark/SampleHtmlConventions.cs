using FubuMVC.Core.UI;

namespace FubuMVC.HelloFubuSpark
{
    public class SampleHtmlConventions : HtmlConventionRegistry
    {
        public SampleHtmlConventions()
        {
            Editors.IfPropertyIs<int>().Modify(tag => tag.AddClass("number"));
        }
    }
}