using System;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Validation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    [TestFixture]
    public class IValidatedRuleSourceTester
    {
        private IValidator theValidator;

        [SetUp]
        public void SetUp()
        {
            theValidator = Validator.BasicValidator();
        }

        [Test]
        public void validator_should_call_through_to_validate_method()
        {
            theValidator.Validate(new ValidatedClass { IsValid = true }).IsValid().ShouldBeTrue();
            theValidator.Validate(new ValidatedClass
            {
                IsValid = false
            }).MessagesFor<ValidatedClass>(x => x.Name)
                .Single().StringToken.ShouldBe(ValidationKeys.Required);
        }
    }

    [TestFixture]
    public class when_building_the_description_for_a_self_validating_class_rule
    {
        private Type theType;
        private SelfValidatingClassRule theRule;
        private Description theDescription;

        [SetUp]
        public void SetUp()
        {
            theType = typeof (ValidatedClass);
            theRule = new SelfValidatingClassRule(theType);

            theDescription = Description.For(theRule);
        }

        [Test]
        public void sets_the_short_description()
        {
            theDescription.ShortDescription.ShouldBe("Self Validating Rule: {0}.Validate".ToFormat(theType.Name));
        }
    }

    public class ValidatedClass : IValidated
    {
        public void Validate(ValidationContext context)
        {
            if (!IsValid)
            {
                context.Notification.RegisterMessage<ValidatedClass>(x => x.Name, ValidationKeys.Required);
            }
        }

        public bool IsValid { get; set; }
        public string Name { get; set; }
    }
}