using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
	public class RegularExpressionFieldRule : IFieldValidationRule
	{
		public RegularExpressionFieldRule(string expression)
			: this(expression, ValidationKeys.RegEx)
		{
		}

		public RegularExpressionFieldRule(string expression, StringToken token)
		{
			Expression = new Regex(expression);
			Token = token;
		}

		public Regex Expression { get; private set; }
		public StringToken Token { get; set; }
		public ValidationMode Mode { get; set; }

		public void Validate(Accessor accessor, ValidationContext context)
		{
			var value = context.GetFieldValue<string>(accessor);
			if (value.IsEmpty())
			{
				return;
			}

			if (!Expression.IsMatch(value))
			{
				context.Notification.RegisterMessage(accessor, Token);
			}
		}
	}
}