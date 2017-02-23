using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class FieldValidationModifierTester
    {
        private RequiredFieldRule theRule;
        private ElementRequest theElementRequest;
        private IValidationAnnotationStrategy theMatchingStrategy;
        private IValidationAnnotationStrategy theOtherStrategy;
        private FieldValidationModifier theModifier;

        public FieldValidationModifierTester()
        {
            theRule = new RequiredFieldRule();
            theElementRequest = ElementRequest.For<FieldValidationModifierTarget>(x => x.Name);
            theMatchingStrategy = MockRepository.GenerateStub<IValidationAnnotationStrategy>();
            theOtherStrategy = MockRepository.GenerateStub<IValidationAnnotationStrategy>();

            theMatchingStrategy.Stub(x => x.Matches(theRule)).Return(true);
            theOtherStrategy.Stub(x => x.Matches(theRule)).Return(false);

            theModifier = new FieldValidationModifier(new[] { theMatchingStrategy, theOtherStrategy });
            theModifier.ModifyFor(theRule, theElementRequest);
        }

        [Fact]
        public void calls_the_matching_strategy()
        {
            theMatchingStrategy.AssertWasCalled(x => x.Modify(theElementRequest, theRule));
        }

        [Fact]
        public void does_not_call_the_other_strategy()
        {
            theOtherStrategy.AssertWasNotCalled(x => x.Modify(theElementRequest, theRule));
        }

        public class FieldValidationModifierTarget
        {
            public string Name { get; set; }
        }
    }
}