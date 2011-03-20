using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    /*
     *  Would like to explicitly register field access rules
     *  Query for field access
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */


    public interface IFieldValidationRule
    {
        void Validate(Accessor accessor, ValidationContext context);
    }

    public interface IFieldValidationSource
    {
        IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property);
    }

    // Could have other adapters.
    public class AttributeFieldValidationSource : IFieldValidationSource
    {
        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            throw new NotImplementedException();
        }
    }


    public class CollectionLengthRule : IFieldValidationRule
    {
        public static readonly string FIELD = "field";
        public static readonly string LENGTH = "length";

        public CollectionLengthRule(int length)
        {
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}