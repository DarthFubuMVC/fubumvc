using FubuMVC.Core.UI;
using FubuMVC.HelloWorld.Conventions;

namespace FubuMVC.HelloWorld
{
    public class SampleHtmlConventions : HtmlConventionRegistry
    {
        public SampleHtmlConventions()
        {
            BeforeEachOfPartial.Builder<BeforeEachOfPartialBuilder>();
            BeforeEachOfPartial.Modifier<OddEvenLiModifier>();
            //BeforeEachOfPartial.If(x => x.Is<ProjectListModel>()).Modify();
            AfterEachOfPartial.Builder<AfterEachOfPartialBuilder>();
            Editors.IfPropertyIs<int>().Modify(tag => tag.AddClass("number"));
        }
    }
}