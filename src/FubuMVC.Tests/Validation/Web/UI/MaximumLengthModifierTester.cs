using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class MaximumLengthModifierTester : ValidationElementModifierContext<MaximumLengthModifier>
    {
        [Fact]
        public void adds_the_maxlength_attribute_for_maximum_length_rule()
        {
            var theRequest = ElementRequest.For(new TargetWithMaxLength(), x => x.Value);
            tagFor(theRequest).Data<object>("maxlength", x => x.ShouldBe("10"));
        }

        [Fact]
        public void no_maxlength_attribute_when_rule_does_not_exist()
        {
            var theRequest = ElementRequest.For(new TargetWithNoMaxLength(), x => x.Value);
            tagFor(theRequest).Attr("maxlength").ShouldBeEmpty();
        }


        public class TargetWithMaxLength
        {
            [MaximumStringLength(10)]
            public string Value { get; set; }
        }

        public class TargetWithNoMaxLength
        {
            public string Value { get; set; }
        }
    }
}