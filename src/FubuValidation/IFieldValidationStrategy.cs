using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation
{
    public interface IFieldValidationStrategy
    {
        IEnumerable<KeyValuePair<string, string>> GetMessageSubstitutions(Accessor accessor);
        ValidationStrategyResult Validate(object target, object rawValue, Type declaringType, Notification notification);
    }
}