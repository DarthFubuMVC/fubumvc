using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation
{
	public class FieldEqualityRule : IValidationRule, DescribesItself
	{
		public static readonly string Field1 = "field1";
		public static readonly string Field2 = "field2";

		private readonly Accessor _property1;
		private readonly Accessor _property2;
		private readonly Type _type;

		private readonly IList<Accessor> _accessors = new List<Accessor>();

		public FieldEqualityRule(Accessor property1, Accessor property2)
			: this(property1, property2, ValidationKeys.FieldEquality)
		{
		}

		public FieldEqualityRule(Accessor property1, Accessor property2, StringToken token)
		{
			_property1 = property1;
			_property2 = property2;

			if (_property1.PropertyType != _property2.PropertyType)
			{
				throw new InvalidOperationException("Property types do not match");
			}
			
			_type = _property1.PropertyType;

			Token = token;
		}

		public Accessor Property1 { get { return _property1; } }
		public Accessor Property2 { get { return _property2; } }

		public StringToken Token { get; set; }

		public void ReportMessagesFor(Accessor accessor)
		{
			if (!_property1.Equals(accessor) && !_property2.Equals(accessor))
			{
				throw new ArgumentOutOfRangeException("accessor", "Property does not exist in the expression");
			}

			_accessors.Fill(accessor);
		}

		public void Validate(ValidationContext context)
		{
			var value1 = _property1.GetValue(context.Target);
			var value2 = _property2.GetValue(context.Target);

			if (value1 == null && value2 == null) return;

			if (_type == typeof(string))
			{
				var str1 = value1 as string;
				var str2 = value2 as string;

				if (str1.IsEmpty() && str2.IsEmpty())
				{
					return;
				}
			}

			if (Equals(value1, value2))
			{
				return;
			}

			_accessors.Each(x =>
			{
				var values = new[]
				{
					TemplateValue.For(Field1, _property1.Name),
					TemplateValue.For(Field2, _property2.Name)
				};

				context
					.Notification
					.RegisterMessage(x, Token, values);
			});
		}

		private IDictionary<string, object> localizedProperty(Accessor accessor)
		{
			var values = new Dictionary<string, object>();
			values.Add("field", accessor.Name);
			values.Add("label", LocalizationManager.GetHeader(accessor.InnerProperty));
			return values;
		}

		public IDictionary<string, object> ToValues()
		{
			var values = new Dictionary<string, object>();
			values.Add("property1", localizedProperty(_property1));
			values.Add("property2", localizedProperty(_property2));

			values.Add("message", Token.ToString());

			values.Add("targets", _accessors.Select(x => x.Name));

			return values;
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "{0}.{1} must equal {0}.{2}".ToFormat(_property1.DeclaringType, _property1.Name, _property2.Name);
		}

		public static FieldEqualityRule For<T>(Expression<Func<T, object>> property1, Expression<Func<T, object>> property2)
		{
			return new FieldEqualityRule(property1.ToAccessor(), property2.ToAccessor());
		}
	}
}