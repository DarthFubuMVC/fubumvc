using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class NumberElementModifierTester : ValidationElementModifierContext<NumberElementModifier>
    {
        [Fact]
        public void adds_the_number_css_class_for_numeric_types()
        {
            tagFor(ElementRequest.For<NumberElementTarget>(x => x.Integer)).HasClass("number").ShouldBeTrue();
            tagFor(ElementRequest.For<NumberElementTarget>(x => x.Double)).HasClass("number").ShouldBeTrue();
            tagFor(ElementRequest.For<NumberElementTarget>(x => x.Decimal)).HasClass("number").ShouldBeTrue();
            tagFor(ElementRequest.For<NumberElementTarget>(x => x.Float)).HasClass("number").ShouldBeTrue();
        }

        [Fact]
        public void no_number_class_for_other_types()
        {
            new NumberElementModifier().Matches(ElementRequest.For<NumberElementTarget>(x => x.NoNumber)).ShouldBeFalse();
        }


        public class NumberElementTarget
        {
            public string NoNumber { get; set; }
            public int Integer { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public float Float { get; set; }
        }
    }
}