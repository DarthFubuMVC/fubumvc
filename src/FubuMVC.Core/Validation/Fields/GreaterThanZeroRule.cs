using System;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class GreaterThanZeroRule : IFieldValidationRule, DescribesItself
    {
	    public GreaterThanZeroRule()
			: this(ValidationKeys.GreaterThanZero)
	    {
	    }

	    public GreaterThanZeroRule(StringToken token)
	    {
		    Token = token;
	    }

	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue == null) return;

            var value = Convert.ToDecimal(rawValue);
            if (value <= 0)
            {
                context.Notification.RegisterMessage(accessor, Token);
            }
        }

        public bool Equals(GreaterThanZeroRule other)
        {
	        return Token.Equals(other.Token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GreaterThanZeroRule)) return false;
            return Equals((GreaterThanZeroRule) obj);
        }

        public override int GetHashCode()
        {
	        return Token.GetHashCode();
        }

	    public void Describe(Description description)
	    {
		    description.ShortDescription = Token.ToString();
	    }
    }
}