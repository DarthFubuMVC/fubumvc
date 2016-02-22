using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
	public class RegularExpressionAttribute : FieldValidationAttribute
	{
		public RegularExpressionAttribute(string expression)
		{
			Expression = expression;
		}

		public string Expression { get; private set; }

		public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
		{
			yield return new RegularExpressionFieldRule(Expression);
		}
	}
}