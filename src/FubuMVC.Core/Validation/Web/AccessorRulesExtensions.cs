using System;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web
{
    // This is all covered by ST
    public static class AccessorRulesExtensions
    {
	    public static void MaximumLength(this IAccessorRulesExpression expression, int length)
        {
            expression.Add(new MaximumLengthRule(length));
        }

		public static void MaximumLength(this IAccessorRulesExpression expression, int length, StringToken message)
		{
			expression.Add(new MaximumLengthRule(length, message));
		}

		public static void MaximumLength(this IAccessorRulesExpression expression, int length, ValidationMode mode)
		{
			expression.Add(new MaximumLengthRule(length) { Mode = mode });
		}

		public static void MaximumLength(this IAccessorRulesExpression expression, int length, StringToken message, ValidationMode mode)
		{
			expression.Add(new MaximumLengthRule(length, message) { Mode = mode});
		}

        public static void GreaterThanZero(this IAccessorRulesExpression expression)
        {
            expression.Add(new GreaterThanZeroRule());
        }

		public static void GreaterThanZero(this IAccessorRulesExpression expression, StringToken message)
		{
			expression.Add(new GreaterThanZeroRule(message));
		}

		public static void GreaterThanZero(this IAccessorRulesExpression expression, ValidationMode mode)
		{
			expression.Add(new GreaterThanZeroRule { Mode = mode });
		}

		public static void GreaterThanZero(this IAccessorRulesExpression expression, StringToken message, ValidationMode mode)
		{
			expression.Add(new GreaterThanZeroRule(message) { Mode = mode });
		}

        public static void GreaterOrEqualToZero(this IAccessorRulesExpression expression)
        {
            expression.Add(new GreaterOrEqualToZeroRule());
        }

		public static void GreaterOrEqualToZero(this IAccessorRulesExpression expression, StringToken message)
		{
			expression.Add(new GreaterOrEqualToZeroRule(message));
		}

		public static void GreaterOrEqualToZero(this IAccessorRulesExpression expression, ValidationMode mode)
		{
			expression.Add(new GreaterOrEqualToZeroRule { Mode = mode });
		}

		public static void GreaterOrEqualToZero(this IAccessorRulesExpression expression, StringToken message, ValidationMode mode)
		{
			expression.Add(new GreaterOrEqualToZeroRule(message) { Mode = mode });
		}

        public static void Required(this IAccessorRulesExpression expression)
        {
            expression.Add(new RequiredFieldRule());
        }

		public static void Required(this IAccessorRulesExpression expression, StringToken message)
		{
			expression.Add(new RequiredFieldRule(message));
		}

		public static void Required(this IAccessorRulesExpression expression, ValidationMode mode)
		{
			expression.Add(new RequiredFieldRule { Mode = mode });
		}

		public static void Required(this IAccessorRulesExpression expression, StringToken message, ValidationMode mode)
		{
			expression.Add(new RequiredFieldRule(message) { Mode = mode });
		}

        public static void Email(this IAccessorRulesExpression expression)
        {
            expression.Add(new EmailFieldRule());
        }

		public static void Email(this IAccessorRulesExpression expression, StringToken message)
		{
			expression.Add(new EmailFieldRule(message));
		}

		public static void Email(this IAccessorRulesExpression expression, ValidationMode mode)
		{
			expression.Add(new EmailFieldRule { Mode = mode });
		}

		public static void Email(this IAccessorRulesExpression expression, StringToken message, ValidationMode mode)
		{
			expression.Add(new EmailFieldRule(message) { Mode = mode });
		}

        public static void MinimumLength(this IAccessorRulesExpression expression, int length)
        {
            expression.Add(new MinimumLengthRule(length));
        }

		public static void MinimumLength(this IAccessorRulesExpression expression, int length, StringToken message)
		{
			expression.Add(new MinimumLengthRule(length, message));
		}

		public static void MinimumLength(this IAccessorRulesExpression expression, int length, ValidationMode mode)
		{
			expression.Add(new MinimumLengthRule(length) { Mode = mode });
		}

		public static void MinimumLength(this IAccessorRulesExpression expression, int length, StringToken message, ValidationMode mode)
		{
			expression.Add(new MinimumLengthRule(length, message) { Mode = mode });
		}

        public static void MinValue(this IAccessorRulesExpression expression, IComparable bounds)
        {
            expression.Add(new MinValueFieldRule(bounds));
        }

		public static void MinValue(this IAccessorRulesExpression expression, IComparable bounds, StringToken message)
		{
			expression.Add(new MinValueFieldRule(bounds, message));
		}

		public static void MinValue(this IAccessorRulesExpression expression, IComparable bounds, ValidationMode mode)
		{
			expression.Add(new MinValueFieldRule(bounds) { Mode = mode });
		}

		public static void MinValue(this IAccessorRulesExpression expression, IComparable bounds, StringToken message, ValidationMode mode)
		{
			expression.Add(new MinValueFieldRule(bounds, message) { Mode = mode });
		}

        public static void RangeLength(this IAccessorRulesExpression expression, int min, int max)
        {
            expression.Add(new RangeLengthFieldRule(min, max));
        }

		public static void RangeLength(this IAccessorRulesExpression expression, int min, int max, StringToken message)
		{
			expression.Add(new RangeLengthFieldRule(min, max, message));
		}

		public static void RangeLength(this IAccessorRulesExpression expression, int min, int max, ValidationMode mode)
		{
			expression.Add(new RangeLengthFieldRule(min, max) { Mode = mode });
		}

		public static void RangeLength(this IAccessorRulesExpression expression, int min, int max, StringToken message, ValidationMode mode)
		{
			expression.Add(new RangeLengthFieldRule(min, max, message) { Mode = mode });
		}

        public static void MaxValue(this IAccessorRulesExpression expression, IComparable bounds)
        {
            expression.Add(new MaxValueFieldRule(bounds));
        }

		public static void MaxValue(this IAccessorRulesExpression expression, IComparable bounds, StringToken message)
		{
			expression.Add(new MaxValueFieldRule(bounds, message));
		}

		public static void MaxValue(this IAccessorRulesExpression expression, IComparable bounds, ValidationMode mode)
		{
			expression.Add(new MaxValueFieldRule(bounds) { Mode = mode });
		}

		public static void MaxValue(this IAccessorRulesExpression expression, IComparable bounds, StringToken message, ValidationMode mode)
		{
			expression.Add(new MaxValueFieldRule(bounds, message) { Mode = mode });
		} 

		public static void ValidationMode(this IAccessorRulesExpression expression, ValidationMode mode)
		{
			expression.Add(mode);
		}

		public static void LiveValidation(this IAccessorRulesExpression expression)
		{
			expression.ValidationMode(Validation.ValidationMode.Live);
		}

		public static void TriggeredValidation(this IAccessorRulesExpression expression)
		{
			expression.ValidationMode(Validation.ValidationMode.Triggered);
		}
    }
}