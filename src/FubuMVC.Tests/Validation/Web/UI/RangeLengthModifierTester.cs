using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class RangeLengthModifierTester : ValidationElementModifierContext<RangeLengthModifier>
    {
        [Fact]
        public void adds_the_rangelength_data_attribute_for_range_length_rule()
        {
            var theRequest = ElementRequest.For(new TargetWithRangeLength(), x => x.Value);
            var values = tagFor(theRequest).Data("rangelength").As<IDictionary<string, object>>();

            values["min"].ShouldBe(5);
            values["max"].ShouldBe(10);
        }

        [Fact]
        public void no_rangelength_data_attribute_when_rule_does_not_exist()
        {
            var theRequest = ElementRequest.For(new TargetWithNoRangeLength(), x => x.Value);
            tagFor(theRequest).Data("rangelength").ShouldBeNull();
        }


        public class TargetWithRangeLength
        {
            [RangeLength(5, 10)]
            public string Value { get; set; }
        }

        public class TargetWithNoRangeLength
        {
            public string Value { get; set; }
        }
    }
}