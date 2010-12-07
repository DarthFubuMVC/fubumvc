using System;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class PartialTagActionExpression
    {
        private readonly PartialTagFactory _factory;
        private readonly Func<AccessorDef, bool> _matches;

        public PartialTagActionExpression(PartialTagFactory factory, Func<AccessorDef, bool> matches)
        {
            _factory = factory;
            _matches = matches;
        }

        private void registerBuilder(EachPartialTagBuilder builder)
        {
            var lambdaBuilder = new PartialLambdaElementBuilder(_matches, def => builder);
            _factory.AddBuilder(lambdaBuilder);
        }

        public void Modify(EachPartialTagModifier modifier)
        {
            var lambdaModifier = new PartialLambdaElementModifier(_matches, def => modifier);
            _factory.AddModifier(lambdaModifier);
        }

        public void Modify(Action<HtmlTag> action)
        {
            EachPartialTagModifier modifier = (request, tag, index, count) => action(tag);
            Modify(modifier);
        }

        public void BuildBy(EachPartialTagBuilder builder)
        {
            registerBuilder(builder);
        }

        public void UseTextbox()
        {
            BuildBy(BuildTextbox);
        }

        public void AddClass(string className)
        {
            Modify((r, tag, index, count) => tag.AddClass(className));
        }

        public void Attr(string attName, object value)
        {
            Modify((r, tag, index, count) => tag.Attr(attName, value));
        }


        public static HtmlTag BuildTextbox(ElementRequest request, int index, int count)
        {
            return new TextboxTag().Attr("value", request.StringValue());
        }

        public static HtmlTag BuildCheckbox(ElementRequest request)
        {
            return new CheckboxTag((bool)request.RawValue);
        }


    }
}