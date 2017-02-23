using FubuCore;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.UI;
using HtmlTags;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class FieldValidationElementModifierTester
    {
        private ElementRequest theRequest;
        private FieldValidationElementModifier theModifier;
        private InMemoryServiceLocator theServices;

        private IFieldValidationModifier theFieldModifier;

        public FieldValidationElementModifierTester()
        {
            theRequest = ElementRequest.For<FieldValidationModifierTarget>(x => x.Name);
            theModifier = new FieldValidationElementModifier();
            theFieldModifier = MockRepository.GenerateStub<IFieldValidationModifier>();

            theRequest.ReplaceTag(new HtmlTag("input"));

            theServices = new InMemoryServiceLocator();
            theServices.Add(ValidationGraph.BasicGraph());
            theServices.Add(theFieldModifier);

            theRequest.Attach(theServices);
        }

        [Fact]
        public void always_matches()
        {
            theModifier.Matches(null).ShouldBeTrue();
        }

        [Fact]
        public void runs_the_modifier_against_the_field_rules()
        {
            theModifier.Modify(theRequest);
            theFieldModifier.AssertWasCalled(x => x.ModifyFor(new RequiredFieldRule(), theRequest));
        }

        public class FieldValidationModifierTarget
        {
            [Required]
            public string Name { get; set; }
        }
    }
}