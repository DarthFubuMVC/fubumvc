using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.UI.Elements.Builders
{
    // Tested through HtmlConventionRegistry
    public class ConditionalElementModifier : IElementModifier, DescribesItself
    {
        private readonly Func<ElementRequest, bool> _filter;
        private readonly IElementModifier _inner;

        public ConditionalElementModifier(Func<ElementRequest, bool> filter, IElementModifier inner)
        {
            _filter = filter;
            _inner = inner;
        }

        public string ConditionDescription { get; set; }



        public bool Matches(ElementRequest token)
        {
            return _filter(token);
        }

        public void Modify(ElementRequest request)
        {
            _inner.Modify(request);
        }

        public void Describe(Description description)
        {
            description.Title = "Conditional Modification";
            description.Properties["Condition"] = ConditionDescription ?? "User defined condition";
            description.Children["Modifier"] = Description.For(_inner);
        }
    }
}