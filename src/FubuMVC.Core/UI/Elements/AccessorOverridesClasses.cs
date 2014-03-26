using System;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Elements
{
    public class AccessorOverrideBuilderPolicy : IElementBuilderPolicy
    {
        public bool Matches(ElementRequest subject)
        {
            var overrides = subject.Get<FubuCore.Reflection.AccessorRules>();


            throw new NotImplementedException();
        }

        public ITagBuilder<ElementRequest> BuilderFor(ElementRequest subject)
        {
            throw new System.NotImplementedException();
        }
    }
}