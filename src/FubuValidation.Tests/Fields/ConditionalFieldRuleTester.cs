using FubuCore.Reflection;
using FubuValidation.Fields;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class ConditionalFieldRuleTester
    {
        private IFieldValidationRule theInnerRule;
        private ConditionalFieldRule<ConditionalFieldRuleTester> theConditionalRule;

        [SetUp]
        public void SetUp()
        {
            theInnerRule = MockRepository.GenerateMock<IFieldValidationRule>();

            theConditionalRule = new ConditionalFieldRule<ConditionalFieldRuleTester>(x => x.Matches, theInnerRule);
        }

        [Test]
        public void execute_the_inner_rule_if_the_condition_is_met()
        {
            Matches = true;
            var context = new ValidationContext(null, new Notification(), this);

            var accessor = ReflectionHelper.GetAccessor<ConditionalFieldRuleTester>(x => x.Matches);

            theConditionalRule.Validate(accessor, context);

            theInnerRule.AssertWasCalled(x => x.Validate(accessor, context));
        }

        [Test]
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