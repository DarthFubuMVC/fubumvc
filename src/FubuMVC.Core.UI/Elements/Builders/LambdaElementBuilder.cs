using System;
using FubuCore.Descriptions;
using HtmlTags;
using HtmlTags.Conventions;
using FubuCore;

namespace FubuMVC.Core.UI.Elements.Builders
{
    //Tested through HtmlConventionRegistry tests
    public class LambdaElementBuilder : LambdaTagBuilder<ElementRequest>, IElementBuilder, DescribesItself
    {
        public LambdaElementBuilder(Func<ElementRequest, HtmlTag> build) : base(build)
        {
        }

        public LambdaElementBuilder(Func<ElementRequest, bool> matcher, Func<ElementRequest, HtmlTag> build) : base(matcher, build)
        {
        }

        public string ConditionDescription { get; set; }
        public string BuilderDescription { get; set; }


        public void Describe(Description description)
        {
            description.Title = "User Defined Builder";

            if (ConditionDescription.IsNotEmpty())
            {
                description.Properties["Condition"] = ConditionDescription;
            }

            if (BuilderDescription.IsNotEmpty())
            {
                description.Properties["Builder"] = BuilderDescription;
            }
        }
    }
}