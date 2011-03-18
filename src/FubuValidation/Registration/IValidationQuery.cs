using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuValidation.Strategies;

namespace FubuValidation.Registration
{
    public interface IValidationQuery
    {
        IEnumerable<IValidationRule> RulesFor(object target);
        T GetRule<T>(Accessor accessor) where T : class, IValidationRule;
        void ForRule<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IValidationRule;
        bool HasRule<T>(Accessor accessor) where T : class, IValidationRule;
    }
}