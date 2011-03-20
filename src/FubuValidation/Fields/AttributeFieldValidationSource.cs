using System.Collections.Generic;
using System.Reflection;
using Enumerable = System.Linq.Enumerable;

namespace FubuValidation.Fields
{
    public class AttributeFieldValidationSource : IFieldValidationSource
    {
        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            return Enumerable.SelectMany<FieldValidationAttribute, IFieldValidationRule>(property.GetAllAttributes<FieldValidationAttribute>(), x => x.RulesFor(property));
        }
    }
}