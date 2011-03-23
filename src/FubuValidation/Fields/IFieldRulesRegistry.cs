using System;
using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public interface IFieldRulesRegistry
    {
        bool HasRule<T>(Accessor accessor) where T : IFieldValidationRule;
        void ForRule<T>(Accessor accessor, Action<T> continuation) where T : IFieldValidationRule;

        void Register(Type type, Accessor accessor, IFieldValidationRule rule);


        ClassFieldValidationRules RulesFor<T>();
        ClassFieldValidationRules RulesFor(Type type);
    }
}