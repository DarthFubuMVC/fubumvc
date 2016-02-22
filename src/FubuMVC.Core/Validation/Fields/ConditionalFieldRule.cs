using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    [IgnoreClientLocalization]
    public class ConditionalFieldRule<T> : IFieldValidationRule, DescribesItself
        where T : class
    {
        private readonly IFieldRuleCondition _condition;
        private readonly IFieldValidationRule _inner;

        public ConditionalFieldRule(IFieldRuleCondition condition, IFieldValidationRule inner)
        {
            _condition = condition;
            _inner = inner;
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            if(_condition.Matches(accessor, context))
            {
                _inner.Validate(accessor, context);
            }
        }

        public IFieldRuleCondition Condition
        {
            get { return _condition; }
        }

	    public StringToken Token
	    {
			get { return _inner.Token; }
			set { _inner.Token = value; }
	    }

	    public ValidationMode Mode
	    {
		    get { return _inner.Mode; }
			set { _inner.Mode = value; }
	    }

        public IFieldValidationRule Inner
        {
            get { return _inner; }
        }

        public void Describe(Description description)
        {
            description.AddChild("If", Condition);
            description.AddChild("Then", Inner);
        }

        public bool Equals(ConditionalFieldRule<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._inner, _inner);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ConditionalFieldRule<T>)) return false;
            return Equals((ConditionalFieldRule<T>) obj);
        }

        public override int GetHashCode()
        {
            return _inner.GetHashCode();
        }

        public static bool operator ==(ConditionalFieldRule<T> left, ConditionalFieldRule<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConditionalFieldRule<T> left, ConditionalFieldRule<T> right)
        {
            return !Equals(left, right);
        }
    }
}