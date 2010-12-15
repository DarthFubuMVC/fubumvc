using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation
{
    public class FieldRule : IValidationRule
    {
        private readonly Accessor _accessor;
        private readonly ITypeResolver _typeResolver;
        private readonly IFieldValidationStrategy _strategy;

        public FieldRule(Accessor accessor, ITypeResolver typeResolver, IFieldValidationStrategy strategy)
        {
            _accessor = accessor;
            _strategy = strategy;
            _typeResolver = typeResolver;
        }

        public IFieldValidationStrategy Strategy { get { return _strategy; } }

    	public bool AppliesTo(Accessor accessor)
    	{
    		return _accessor.Equals(accessor);
    	}

    	public void Validate(object target, Notification notification)
        {
            var declaringType = _typeResolver.ResolveType(target);
            var rawValue = _accessor.GetValue(target);

            var result = _strategy.Validate(target, rawValue, declaringType, notification);
            if(result.IsValid)
            {
                return;
            }

            _strategy
                .GetMessageSubstitutions(_accessor)
                .Each(pair => result.Message.AddSubstitution(pair.Key, pair.Value));

            notification
                .RegisterMessage(_accessor, result.Message);
        }
    }
}