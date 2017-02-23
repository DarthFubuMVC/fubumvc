using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
	
	public class ValidationModeAttributePolicyTester
	{
		private Accessor accessorFor<T>(Expression<Func<T, object>> expression)
		{
			return SingleProperty.Build(expression);
		}

		private bool matches<T>(Expression<Func<T, object>> expression)
		{
			var accessor = accessorFor(expression);
			return new ValidationModeAttributePolicy().Matches(new InMemoryServiceLocator(), accessor);
		}

		[Fact]
		public void matches_accessors_for_properties_with_live_validation_attribute()
		{
			matches<ValidationModeAttributeTarget>(x => x.Live).ShouldBeTrue();
		}

		[Fact]
		public void matches_accessors_for_properties_with_triggered_validation_attribute()
		{
			matches<ValidationModeAttributeTarget>(x => x.Triggered).ShouldBeTrue();
		}

		[Fact]
		public void matches_accessors_for_properties_belonging_to_a_class_with_validation_attribute()
		{
			matches<ClassLevelValidationModeAttributeTarget>(x => x.Property).ShouldBeTrue();
		}

		[Fact]
		public void matches_accessors_for_properties_with_no_validation_mode_attributes()
		{
			matches<ValidationModeAttributeTarget>(x => x.Ignored).ShouldBeFalse();
		}

		[Fact]
		public void mode_is_determined_from_live_attribute()
		{
			var accessor = accessorFor<ValidationModeAttributeTarget>(x => x.Live);
			new ValidationModeAttributePolicy()
				.DetermineMode(new InMemoryServiceLocator(), accessor)
				.ShouldBe(ValidationMode.Live);
		}


		[Fact]
		public void mode_is_determined_from_triggered_attribute()
		{
			var accessor = accessorFor<ValidationModeAttributeTarget>(x => x.Triggered);
			new ValidationModeAttributePolicy()
				.DetermineMode(new InMemoryServiceLocator(), accessor)
				.ShouldBe(ValidationMode.Triggered);
		}

		[Fact]
		public void mode_is_determined_from_class_level_attribute()
		{
			var accessor = accessorFor<ClassLevelValidationModeAttributeTarget>(x => x.Property);
			new ValidationModeAttributePolicy()
				.DetermineMode(new InMemoryServiceLocator(), accessor)
				.ShouldBe(ValidationMode.Triggered);
		}

		public class ValidationModeAttributeTarget
		{
			[TriggeredValidation]
			public string Triggered { get; set; }

			[LiveValidation]
			public string Live { get; set; }

			public string Ignored { get; set; }
		}

		[TriggeredValidation]
		public class ClassLevelValidationModeAttributeTarget
		{
			public string Property { get; set; }
		}
	}
}