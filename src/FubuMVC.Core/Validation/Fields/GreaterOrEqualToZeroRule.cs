using System;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class GreaterOrEqualToZeroRule : IFieldValidationRule, DescribesItself
    {
	    public GreaterOrEqualToZeroRule()
			: this(ValidationKeys.GreaterThanOrEqualToZero)
	    {
	    }

	    public GreaterOrEqualToZeroRule(StringToken token)
	    {
		    Token = token;
	    }

	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue != null)
            {
                var value = Convert.ToDouble(rawValue);
                if (value < 0)
                {
                    context.Notification.RegisterMessage(accessor, Token);
                }
            }
        }

        public bool Equals(GreaterOrEqualToZeroRule other)
        {
	        return Token.Equals(other.Token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GreaterOrEqualToZeroRule)) return false;
            return Equals((GreaterOrEqualToZeroRule) obj);
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