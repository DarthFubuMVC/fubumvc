using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Validation
{
    public class ValidationStep : DescribesItself
    {
        private readonly Type _type;
        private readonly Type _source;
        private readonly IEnumerable<IValidationRule> _rules;

        public ValidationStep(Type type, Type source, IEnumerable<IValidationRule> rules)
        {
            _type = type;
            _source = source;
            _rules = rules;
        }

        public Type Type
        {
            get { return _type; }
        }

        public Type Source { get { return _source; } }

        public IEnumerable<IValidationRule> Rules
        {
            get { return _rules; }
        }

		public IEnumerable<T> FindRules<T>()
			where T : IValidationRule
		{
			return _rules.OfType<T>();
		}

        public void Execute(ValidationContext context)
        {
            // TODO -- Could push a logging mechanism into the context
            _rules.Each(x => x.Validate(context));
        }

        void DescribesItself.Describe(Description description)
        {
            var list = description.AddList("ValidationRules", _rules);
            list.Label = "Validation Rules";
            list.IsOrderDependent = true;
        }

        public override string ToString()
        {
            return "Validate {0} from {1}".ToFormat(_type.Name, _source.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ValidationStep)) return false;
            return Equals((ValidationStep) obj);
        }

        public bool Equals(ValidationStep other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._type == _type && other._source == _source && _rules.SequenceEqual(other._rules);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _type.GetHashCode();
                result = (result*397) ^ _source.GetHashCode();
                result = (result*397) ^ _rules.GetHashCode();
                return result;
            }
        }

		public static ValidationStep FromSource(Type type, IValidationSource source)
		{
			return new ValidationStep(type, source.GetType(), source.RulesFor(type));
		}
    }
}