using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Validation.Fields
{
    public class FieldRuleSource : IValidationSource
    {
        private readonly IFieldRulesRegistry _registry;

        public FieldRuleSource(IFieldRulesRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            yield return _registry.RulesFor(type);
        }
    }
}