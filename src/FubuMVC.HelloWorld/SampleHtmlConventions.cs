using FubuMVC.HelloWorld.Conventions;
using FubuMVC.UI;

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

        }
    }
}