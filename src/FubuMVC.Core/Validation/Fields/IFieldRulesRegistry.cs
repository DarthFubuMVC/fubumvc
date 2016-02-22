using System;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
	// SAMPLE: IFieldRulesRegistry
    public interface IFieldRulesRegistry
    {
        bool HasRule<T>(Accessor accessor) where T : IFieldValidationRule;
        void ForRule<T>(Accessor accessor, Action<T> continuation) where T : IFieldValidationRule;

        void Register(Type type, Accessor accessor, IFieldValidationRule rule);

	    void FindWith(IFieldValidationSource source);

        ClassFieldValidationRules RulesFor<T>();
        ClassFieldValidationRules RulesFor(Type type);
    }
	// ENDSAMPLE
}