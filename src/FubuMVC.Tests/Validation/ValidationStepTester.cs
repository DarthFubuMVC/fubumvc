using System;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class ValidationStepTester
    {
        private ValidationStep theStep;
        private object theModel;
        private Type theSource;
        private IValidationRule r1;
        private IValidationRule r2;
        private ValidationContext theContext;

        public ValidationStepTester()
        {
            theModel = new SimpleModel();

            r1 = MockRepository.GenerateStub<IValidationRule>();
            r2 = MockRepository.GenerateStub<IValidationRule>();
            theSource = typeof(ConfiguredValidationSource);

            theContext = ValidationContext.For(theModel);

            theStep = new ValidationStep(theModel.GetType(), theSource, new[] { r1, r2 });
        }

        [Fact]
        public void the_source()
        {
            theStep.Source.ShouldBe(theSource);
        }

        [Fact]
        public void sets_the_rules()
        {
            theStep.Rules.ShouldHaveTheSameElementsAs(r1, r2);
        }

        [Fact]
        public void executes_each_rule()
        {
            theStep.Execute(theContext);

            r1.AssertWasCalled(x => x.Validate(theContext));
            r2.AssertWasCalled(x => x.Validate(theContext));
        }

		[Fact]
		public void finds_the_rules()
		{
			var r1 = new StubRule();
			var r2 = new StubRule();
			var r3 = new ClassFieldValidationRules();

			var src = new ConfiguredValidationSource(new IValidationRule[] {r1, r2, r3});
			var step = ValidationStep.FromSource(typeof (object), src);

			step.FindRules<StubRule>().ShouldHaveTheSameElementsAs(r1, r2);
		}
    }

	public class StubRule : IValidationRule
	{
		public void Validate(ValidationContext context)
		{
			throw new NotImplementedException();
		}
	}

    
    public class when_building_the_description_for_the_validation_step
    {
        private ValidationStep theStep;
        private Description theDescription;
        private BulletList theRuleList;

        public when_building_the_description_for_the_validation_step()
        {
            theStep = new ValidationStep(typeof(string), typeof(ConfiguredValidationSource), new IValidationRule[] { new Rule1(), new Rule2()  });

            theDescription = Description.For(theStep);
            theRuleList = theDescription.BulletLists.Single();
        }

        [Fact]
        public void the_short_description_of_the_step()
        {
            theDescription.ShortDescription.ShouldBe("Validate {0} from {1}".ToFormat(typeof(string).Name, typeof(ConfiguredValidationSource).Name));
        }

        [Fact]
        public void the_name_and_the_label_of_the_validation_rule_list()
        {
            theRuleList.Name.ShouldBe("ValidationRules");
            theRuleList.Label.ShouldBe("Validation Rules");
        }

        [Fact]
        public void the_validation_rules_list_must_be_marked_as_order_dependent()
        {
            theRuleList.IsOrderDependent.ShouldBeTrue();
        }

        public class Rule1 : IValidationRule
        {
            public void Validate(ValidationContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class Rule2 : IValidationRule
        {
            public void Validate(ValidationContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}