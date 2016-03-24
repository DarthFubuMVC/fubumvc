using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web
{
    public class AccessorRulesFieldSource : IFieldValidationSource
    {
        private readonly AccessorRules _rules;

        public AccessorRulesFieldSource(AccessorRules rules)
        {
            _rules = rules;
        }

        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            return _rules.AllRulesFor<IFieldValidationRule>(new SingleProperty(property));
        }

        public void AssertIsValid()
        {
        }
    }
}