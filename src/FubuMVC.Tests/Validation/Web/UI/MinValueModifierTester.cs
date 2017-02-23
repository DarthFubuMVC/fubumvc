using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class MinValueModifierTester : ValidationElementModifierContext<MinValueModifier>
    {
        [Fact]
        public void adds_the_min_data_attribute_for_min_value_rule()
        {
            var theRequest = ElementRequest.For(new TargetWithMinValue(), x => x.Value);
            tagFor(theRequest).Data("min").ShouldBe(10);
        }

        [Fact]
        public void no_data_attribute_when_rule_does_not_exist()
        {
            var theRequest = ElementRequest.For(new TargetWithNoMinValue(), x => x.Value);
            tagFor(theRequest).Data("min").ShouldBeNull();
        }


        public class TargetWithMinValue
        {
            [MinValue(10)]
            public string Value { get; set; }
        }

        public class TargetWithNoMinValue
        {
            public string Value { get; set; }
        }
    }
}