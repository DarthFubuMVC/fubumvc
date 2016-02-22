using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Validation
{
	// SAMPLE: IValidated
    public interface IValidated
    {
        void Validate(ValidationContext context);
    }
	// ENDSAMPLE

    public class SelfValidatingClassRule : IValidationRule
    {
        private readonly Type _type;

        public SelfValidatingClassRule(Type type)
        {
            _type = type;
        }

        public void Validate(ValidationContext context)
        {
            context.Target.As<IValidated>().Validate(context);
        }

        public override bool Equals(object obj)
        {
            var rule = obj as SelfValidatingClassRule;
            if (rule == null) return false;

            return Equals(rule);
        }

        public override string ToString()
        {
            return "Self Validating Rule: {0}.Validate".ToFormat(_type.Name);
        }

        public bool Equals(SelfValidatingClassRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._type == _type;
        }

        public override int GetHashCode()
        {
            return (_type != null ? _type.GetHashCode() : 0);
        }
    }

	// SAMPLE: SelfValidatingClassRuleSource
    public class SelfValidatingClassRuleSource : IValidationSource
    {
        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            if (type.CanBeCastTo<IValidated>())
            {
                yield return new SelfValidatingClassRule(type);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SelfValidatingClassRuleSource;
        }
    }
	// ENDSAMPLE
}