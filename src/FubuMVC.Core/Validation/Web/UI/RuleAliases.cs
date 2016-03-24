using System;
using FubuCore.Util;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
	public class RuleAliases
	{
		private static readonly Cache<Type, string> Aliases = new Cache<Type, string>(createRuleAlias);

		private static string createRuleAlias(Type type)
		{
			// RequiredFieldRule => required
			return type.Name.Replace("Field", "").Replace("Rule", "").ToLower();
		}

		public static void RegisterAlias<T>(string alias)
			where T : IFieldValidationRule
		{
			var type = typeof (T);
			if (Aliases.Has(type))
			{
				Aliases.Remove(type);
			}

			Aliases.Fill(type, alias);
		}

		public static string AliasFor(IFieldValidationRule rule)
		{
			return AliasFor(rule.GetType());
		}

		public static string AliasFor(Type type)
		{
			return Aliases[type];
		}
	}
}