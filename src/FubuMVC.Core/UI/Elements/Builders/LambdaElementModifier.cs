using System;
using FubuCore.Descriptions;
using HtmlTags.Conventions;
using FubuCore;

namespace FubuMVC.Core.UI.Elements.Builders
{
    // Tested through HtmlConventionRegistry
    public class LambdaElementModifier : LambdaTagModifier<ElementRequest>, IElementModifier, DescribesItself
    {
        public LambdaElementModifier(Func<ElementRequest, bool> matcher, Action<ElementRequest> modify) : base(matcher, modify)
        {
        }

        public LambdaElementModifier(Action<ElementRequest> modify) : base(modify)
        {
        }

        public string ConditionDescription { get; set; }
        public string ModifierDescription { get; set; }


        public void Describe(Description description)
        {
            description.Title = "User Defined Modifier";

            if (ConditionDescription.IsNotEmpty())
            {
                description.Properties["Condition"] = ConditionDescription;
            }

            if (ModifierDescription.IsNotEmpty())
            {
                description.Properties["Modifier"] = ModifierDescription;
            }
        }
    }
}