using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
    public class AttributeFieldValidationSource : IFieldValidationSource
    {
        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            return property.GetAllAttributes<FieldValidationAttribute>()
                .SelectMany(x => x.RulesFor(property));
        }

        public void AssertIsValid()
        {
            
        }

        public override bool Equals(object obj)
        {
            return obj is AttributeFieldValidationSource;
        }
    }
}