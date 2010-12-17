using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Registration
{
    public interface IValidationQuery
    {
        IEnumerable<IValidationRule> RulesFor(object target);
        T GetRule<T>(Accessor accessor) where T : class, IValidationRule;
        T GetStrategy<T>(Accessor accessor) where T : class, IFieldValidationStrategy;
        void ForRule<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IValidationRule;
        void ForStrategy<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IFieldValidationStrategy;
        bool HasRule<T>(Accessor accessor) where T : class, IValidationRule;
		bool HasStrategy<T>(Accessor accessor) where T : class, IFieldValidationStrategy;
    }
}