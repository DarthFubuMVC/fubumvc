using System;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    [TestFixture]
    public class ValidationPlanTester
    {
        private ValidationPlan thePlan;
        private ValidationGraph theGraph;
        private ConfiguredValidationSource theMatchingSource;
        private ConfiguredValidationSource theOtherSource;

        private Type theType;

        private IValidationRule r1;
        private IValidationRule r2;
        private IValidationRule r3;

        private object theModel;
        private ValidationContext theContext;

        [SetUp]
        public void SetUp()
        {
            theModel = new ContactModel();
            theType = theModel.GetType();

            r1 = MockRepository.GenerateStub<IValidationRule>();
            r2 = MockRepository.GenerateStub<IValidationRule>();
            r3 = MockRepository.GenerateStub<IValidationRule>();

            theMatchingSource = ConfiguredValidationSource.For(type => type == theType, r1, r2);
            theOtherSource = ConfiguredValidationSource.For(type => type == typeof(int), r3);

            theGraph = ValidationGraph.BasicGraph();
            theGraph.RegisterSource(theMatchingSource);
            theGraph.RegisterSource(theOtherSource);

            theContext = ValidationContext.For(theModel);

            thePlan = ValidationPlan.For(theType, theGraph);
        }

        [Test]
        public void creates_a_step_for_each_rule()
        {
            Debug.WriteLine(thePlan.ToDescriptionText());
            var theStep = thePlan.Steps.Single(x => x.Source == typeof(ConfiguredValidationSource));
            theStep.Rules.ShouldHaveTheSameElementsAs(r1, r2);
        }

        [Test]
        public void executes_each_step()
        {
            thePlan.Execute(theContext);

            r1.AssertWasCalled(x => x.Validate(theContext));
            r2.AssertWasCalled(x => x.Validate(theContext));
        }

        [Test]
        public void builds_up_the_field_rules()
        {
            thePlan.FieldRules.HasRule<RequiredFieldRule>(ReflectionHelper.GetAccessor<ContactModel>(x => x.FirstName)).ShouldBeTrue();
        }

		[Test]
		public void finds_the_rules_from_all_steps()
		{
			var r1 = new StubRule();
			var r2 = new StubRule();
			var r3 = new ClassFieldValidationRules();
			var r4 = new ClassFieldValidationRules();

			var src1 = new ConfiguredValidationSource(new IValidationRule[] { r1, r3 });
			var src2 = new ConfiguredValidationSource(new IValidationRule[] { r2, r4 });

			var step1 = ValidationStep.FromSource(typeof(object), src1);
			var step2 = ValidationStep.FromSource(typeof(object), src2);

			var plan = new ValidationPlan(typeof (object), new[] {step1, step2});
			plan.FindRules<StubRule>().ShouldHaveTheSameElementsAs(r1, r2);
		}
    }

    [TestFixture]
    public class when_building_the_description_for_the_validation_plan
    {
        private ValidationPlan thePlan;
        private Description theDescription;
        private BulletList theStepList;

        [SetUp]
        public void SetUp()
        {
            thePlan = new ValidationPlan(typeof(string), new ValidationStep[0]);

            theDescription = Description.For(thePlan);
            theStepList = theDescription.BulletLists.Single();
        }

        [Test]
        public void the_short_description_of_the_plan()
        {
            theDescription.ShortDescription.ShouldBe("Validate {0}".ToFormat(typeof (string).Name));
        }

        [Test]
        public void the_name_and_the_label_of_the_validation_step_list()
        {
            theStepList.Name.ShouldBe("ValidationSteps");
            theStepList.Label.ShouldBe("Validation Steps");
        }

        [Test]
        public void the_validation_step_list_must_be_marked_as_order_dependent()
        {
            theStepList.IsOrderDependent.ShouldBeTrue();
        }
    }
}