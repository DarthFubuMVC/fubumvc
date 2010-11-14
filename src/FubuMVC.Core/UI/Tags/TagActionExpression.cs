using System;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class TagActionExpression
    {
        private readonly TagFactory _factory;
        private readonly Func<AccessorDef, bool> _matches;

        public TagActionExpression(TagFactory factory, Func<AccessorDef, bool> matches)
        {
            _factory = factory;
            _matches = matches;
        }

        private void registerBuilder(TagBuilder builder)
        {
            var lambdaBuilder = new LambdaElementBuilder(_matches, def => builder);
            _factory.AddBuilder(lambdaBuilder);
        }

        public void Modify(TagModifier modifier)
        {
            var lambdaModifier = new LambdaElementModifier(_matches, def => modifier);
            _factory.AddModifier(lambdaModifier);
        }

        public void Modify(Action<HtmlTag> action)
        {
            TagModifier modifier = (request, tag) => action(tag);
            Modify(modifier);
        }

        public void BuildBy(TagBuilder builder)
        {
            registerBuilder(builder);
        }

        public void UseTextbox()
        {
            BuildBy(BuildTextbox);
        }

        public void AddClass(string className)
        {
            Modify((r, tag) => tag.AddClass(className));
        }

        public void Attr(string attName, object value)
        {
            Modify((r, tag) => tag.Attr(attName, value));
        }


        public static HtmlTag BuildTextbox(ElementRequest request)
        {
            return new TextboxTag().Attr("value", request.StringValue());
        }

        public static HtmlTag BuildCheckbox(ElementRequest request)
        {
            return new CheckboxTag((bool) request.RawValue);
        }
    }
}