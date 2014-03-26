using System;
using FubuCore.Descriptions;
using HtmlTags;
using HtmlTags.Conventions;
using FubuCore;

namespace FubuMVC.Core.UI.Elements.Builders
{
    //Tested through HtmlConventionRegistry tests
    public class LambdaElementBuilder : TagBuilder<ElementRequest>, DescribesItself
    {
        private readonly Func<ElementRequest, bool> _matcher;
        private readonly Func<ElementRequest, HtmlTag> _build;

        public LambdaElementBuilder(Func<ElementRequest, HtmlTag> build) : this(x => true,build)
        {
            ConditionDescription = "Always";
        }

        public LambdaElementBuilder(Func<ElementRequest, bool> matcher, Func<ElementRequest, HtmlTag> build)
        {
            _matcher = matcher;
            _build = build;
        }

        public string ConditionDescription { get; set; }
        public string BuilderDescription { get; set; }


        public void Describe(Description description)
        {
            description.Title = "User Defined BuilderPolicy";

            if (ConditionDescription.IsNotEmpty())
            {
                description.Properties["Condition"] = ConditionDescription;
            }

            if (BuilderDescription.IsNotEmpty())
            {
                description.Properties["BuilderPolicy"] = BuilderDescription;
            }
        }


        public override bool Matches(ElementRequest subject)
        {
            return _matcher(subject);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return _build(request);
        }
    }
}