using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuMVC.Core.Validation.Fields
{
	public class ClassFieldValidationRules : IValidationRule, DescribesItself
	{
		private readonly Cache<Accessor, IList<IFieldValidationRule>> _rules =
			new Cache<Accessor, IList<IFieldValidationRule>>(a => new List<IFieldValidationRule>());

		public void Validate(ValidationContext context)
		{
			_rules.Each((accessor, rules) => rules.Each(rule => rule.Validate(accessor, context)));
		}

		public void AddRule(Accessor accessor, IFieldValidationRule rule)
		{
			_rules[accessor].Add(rule);
		}

		public void AddRules(Accessor accessor, IEnumerable<IFieldValidationRule> rules)
		{
			_rules[accessor].AddRange(rules);
		}

		public bool HasRule<T>(Accessor accessor) where T : IFieldValidationRule
		{
			return _rules[accessor].Any(x => x is T);
		}

		public IEnumerable<IFieldValidationRule> RulesFor(Accessor accessor)
		{
			return _rules[accessor];
		}

		public IEnumerable<IFieldValidationRule> RulesFor<T>(Expression<Func<T, object>> property)
		{
			return _rules[property.ToAccessor()];
		}

		public void ForRule<T>(Accessor accessor, Action<T> continuation) where T : IFieldValidationRule
		{
			_rules[accessor].OfType<T>().Each(continuation);
		}

		public void Describe(Description description)
		{
			var items = new List<FieldRuleListItem>();
			_rules.Each((accessor, rules) =>
			{
				items.AddRange(rules.Select(rule => new FieldRuleListItem {Accessor = accessor, Rule = rule}));
			});

			var list = description.AddList("FieldRules", items);
			list.Label = "Field Validation Rules";
			list.IsOrderDependent = true;
		}

		public class FieldRuleListItem : DescribesItself
		{
			public Accessor Accessor { get; set; }
			public IFieldValidationRule Rule { get; set; }

			public void Describe(Description description)
			{
				description.Title = "{0}: {1}".ToFormat(Accessor.Name, Rule.GetType().Name);
				if (Rule.GetType().CanBeCastTo<DescribesItself>())
				{
					Rule.As<DescribesItself>().Describe(description);
				}
			}
		}
	}
}