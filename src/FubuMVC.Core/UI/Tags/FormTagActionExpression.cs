using System;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class FormTagActionExpression
    {
        private readonly FormTagFactory _factory;
        private readonly Func<FormDef, bool> _matches;

        public FormTagActionExpression(FormTagFactory factory, Func<FormDef, bool> matches)
        {
            _factory = factory;
            _matches = matches;
        }

        public void Modify(FormTagModifier modifier)
        {
            var lambdaModifier = new FormLambdaElementModifier(_matches, def => modifier);
            _factory.AddModifier(lambdaModifier);
        }

        public void Modify(Action<FormTag> action)
        {
            FormTagModifier modifier = (request, tag) => action(tag);
            Modify(modifier);
        }

        public void AddClass(string className)
        {
            Modify((r, tag) => tag.AddClass(className));
        }

        public void Attr(string attName, object value)
        {
            Modify((r, tag) => tag.Attr(attName, value));
        }
    }
}