using System;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation
{
    public abstract class FieldMarkerAttribute: ValidationAttribute
    {
        private readonly Type _strategyType;

        public FieldMarkerAttribute(Type strategyType)
        {
            if(!typeof(IFieldValidationStrategy).IsAssignableFrom(strategyType))
            {
                throw new ArgumentException("Must be assignable from IFieldValidationStraetgy", "strategyType");
            }

            _strategyType = strategyType;
        }

        public override IValidationRule CreateRule(Accessor accessor)
        {
            var strategy = Activator
                           .CreateInstance(_strategyType)
                           .As<IFieldValidationStrategy>();

            visitStrategy(strategy);

            return new FieldRule(accessor, new TypeResolver(), strategy);
        }

        protected virtual void visitStrategy(IFieldValidationStrategy strategy)
        {
        }
    }
}