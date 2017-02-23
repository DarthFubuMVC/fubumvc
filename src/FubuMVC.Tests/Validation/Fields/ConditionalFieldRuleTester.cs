using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class ConditionalFieldRuleTester
    {
        private IFieldValidationRule theInnerRule;
        private ConditionalFieldRule<ConditionalFieldRuleTester> theConditionalRule;

        public ConditionalFieldRuleTester()
        {
            var condition = FieldRuleCondition.For<ConditionalFieldRuleTester>(x => x.Matches);
            
            theInnerRule = MockRepository.GenerateMock<IFieldValidationRule>();
            theConditionalRule = new ConditionalFieldRule<ConditionalFieldRuleTester>(condition, theInnerRule);
        }

        [Fact]
        public void execute_the_inner_rule_if_the_condition_is_met()
        {
            Matches = true;
            var context = new ValidationContext(null, new Notification(), this);

            var accessor = ReflectionHelper.GetAccessor<ConditionalFieldRuleTester>(x => x.Matches);

            theConditionalRule.Validate(accessor, context);

            theInnerRule.AssertWasCalled(x => x.Validate(accessor, context));
        }

        [Fact]
        public void should_not_execute_the_inner_rule_if_the_condition_is_not_met()
        {
            Matches = false;
            var context = new ValidationContext(null, new Notification(), this);

            var accessor = ReflectionHelper.GetAccessor<ConditionalFieldRuleTester>(x => x.Matches);

            theConditionalRule.Validate(accessor, context);

            theInnerRule.AssertWasNotCalled(x => x.Validate(accessor, context));
        }

        public bool Matches { get; set; }
    }
}