using System;
using FubuCore.Descriptions;
using HtmlTags;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Elements.Builders
{
    // Tested through HtmlConventionRegistry
    public class ConditionalElementBuilder : IElementBuilderPolicy, DescribesItself, ITagBuilder<ElementRequest>
    {
        private readonly Func<ElementRequest, bool> _filter;
        private readonly IElementBuilder _inner;

        public ConditionalElementBuilder(Func<ElementRequest, bool> filter, IElementBuilder inner)
        {
            _filter = filter;
            _inner = inner;
        }

        public string ConditionDescription { get; set; }
        public bool Matches(ElementRequest subject)
        {
            return _filter(subject);
        }

        public ITagBuilder<ElementRequest> BuilderFor(ElementRequest subject)
        {
            return this;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return _inner.Build(request);
        }

        public void Describe(Description description)
        {
            description.Title = "Conditional Modification";
            description.Properties["Condition"] = ConditionDescription ?? "User defined condition";
            description.Children["BuilderPolicy"] = Description.For(_inner);
        }
    }
}