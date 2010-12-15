using FubuMVC.Core.UI;
using FubuMVC.HelloWorld.Conventions;
using FubuValidation;
using FubuValidation.Strategies;


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

            //Editors.ModifyForStrategy<RequiredFieldStrategy>((tag, strategy, accessor) => tag.AddClass("required"));
            //Editors.ModifyForStrategy<MaximumStringLengthFieldStrategy>((tag, strategy, accessor) => tag.Attr("maxlength", strategy.Length));
        }
    }
}