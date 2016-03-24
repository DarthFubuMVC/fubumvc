using System.Collections.Generic;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class ElementLocalizationMessages
	{
		private readonly IDictionary<string, string> _messages = new Dictionary<string, string>();
		
		public IDictionary<string, string> Messages { get { return _messages; } }

		public void Add(IFieldValidationRule rule)
		{
			var key = RuleAliases.AliasFor(rule);
			_messages.Fill(key, rule.Token.ToString());
		}
	}
}