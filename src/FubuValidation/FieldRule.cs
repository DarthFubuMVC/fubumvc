using FubuCore;
using FubuCore.Reflection;
using FubuValidation.Strategies;

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

    	public void Validate(object target, ValidationContext context, Notification notification)
        {
            var declaringType = _typeResolver.ResolveType(target);
            var rawValue = _accessor.GetValue(target);

    	    var strategyContext = new ValidationStrategyContext(target, rawValue, declaringType, context.Provider, notification);
            var result = _strategy.Validate(strategyContext);
            if(result.IsValid)
            {
                return;
            }

            _strategy
                .GetMessageSubstitutions(_accessor)
                .Each((key, value) => result.Message.AddSubstitution(key, value));

            notification
                .RegisterMessage(_accessor, result.Message);
        }
    }
}